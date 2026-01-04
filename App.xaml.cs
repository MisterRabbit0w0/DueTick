using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using DueTick.Models;
using DueTick.Services;
using DueTick.Utils;
using DueTick.Views;
using Application = System.Windows.Application;

namespace DueTick
{
    public partial class App : Application
    {
        private static readonly string MutexName = "DueTick_SingleInstance_Mutex";
        private Mutex _mutex;

        private NotifyIcon _notifyIcon;
        private StorageService _storage;
        private List<Deadline> _deadlines;
        private Settings _settings;
        private ClipboardMonitor _clipboardMonitor;
        private TimerService _timerService;
        private WidgetWindow _widgetWindow;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Check for single instance
            _mutex = new Mutex(true, MutexName, out bool createdNew);
            if (!createdNew)
            {
                // Another instance is running, exit
                Shutdown();
                return;
            }

            _storage = new StorageService();
            _deadlines = _storage.LoadDeadlines();
            _settings = _storage.LoadSettings();

            InitializeTrayIcon();
            NotificationManager.Initialize(_notifyIcon);
            InitializeServices();

            if (_settings.ShowWidget)
            {
                ShowWidgetWindow();
            }
        }

        private void InitializeTrayIcon()
        {
            Icon trayIcon;
            try
            {
                // Load the embedded icon.png as tray icon
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("DueTick.Resources.icon.png"))
                {
                    if (stream != null)
                    {
                        using (var originalBitmap = new Bitmap(stream))
                        {
                            // choose 64x64 mapping for high DPI support
                            using (var resizedBitmap = new Bitmap(originalBitmap, new System.Drawing.Size(64, 64)))
                            {
                                trayIcon = Icon.FromHandle(resizedBitmap.GetHicon());
                            }
                        }
                    }
                    else
                    {
                        trayIcon = SystemIcons.Application;
                    }
                }
            }
            catch
            {
                trayIcon = SystemIcons.Application;
            }

            _notifyIcon = new System.Windows.Forms.NotifyIcon
            {
                Icon = trayIcon,
                Visible = true,
                Text = "DueTick - Deadline Reminder"
            };

            _notifyIcon.DoubleClick += (s, e) => ShowWidgetWindow();
            _notifyIcon.ContextMenuStrip = CreateContextMenu();
        }

        private ContextMenuStrip CreateContextMenu()
        {
            var menu = new ContextMenuStrip();

            var addItem = new ToolStripMenuItem("Add Deadline", null, (s, e) => ShowAddDeadlineDialog());
            var viewItem = new ToolStripMenuItem("View Deadlines", null, (s, e) => ShowWidgetWindow());
            var settingsItem = new ToolStripMenuItem("Settings", null, (s, e) => ShowSettingsDialog());
            var exitItem = new ToolStripMenuItem("Exit", null, (s, e) => Shutdown());

            menu.Items.AddRange(new ToolStripItem[] { addItem, viewItem, settingsItem, new ToolStripSeparator(), exitItem });

            return menu;
        }

        private void InitializeServices()
        {
            _clipboardMonitor = new ClipboardMonitor();
            _clipboardMonitor.ClipboardChanged += OnClipboardChanged;

            _timerService = new TimerService();
            _timerService.Tick += CheckReminders;
            _timerService.Start();
        }

        private void OnClipboardChanged(object sender, string text)
        {
            if (!_settings.EnableClipboardMonitoring) return;

            var parser = new DateParser();
            var detected = parser.Parse(text);

            if (detected.HasValue)
            {
                var result = System.Windows.MessageBox.Show(
                    $"Detected deadline: {detected.Value.title} at {detected.Value.date}\nSave it?",
                    "DueTick - Clipboard Detection",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _deadlines.Add(new Deadline
                    {
                        Title = detected.Value.title,
                        DueDate = detected.Value.date
                    });
                    SaveDeadlines();
                    UpdateWidget();
                }
            }
        }

        private void CheckReminders(object sender, EventArgs e)
        {
            var toNotify = _deadlines.Where(d => d.ShouldNotify).ToList();
            foreach (var deadline in toNotify)
            {
                NotificationManager.ShowNotification(deadline);
                deadline.IsCompleted = true; // Prevent repeated notifications
            }

            if (toNotify.Any())
            {
                SaveDeadlines();
                UpdateWidget();
            }
        }

        private void ShowAddDeadlineDialog()
        {
            var dialog = new AddDeadlineDialog(_settings.PresetOffsets);
            if (dialog.ShowDialog() == true)
            {
                _deadlines.Add(dialog.Deadline);
                SaveDeadlines();
                UpdateWidget();
            }
        }

        private void ShowSettingsDialog()
        {
            var dialog = new SettingsDialog(_settings);
            if (dialog.ShowDialog() == true)
            {
                _settings = dialog.Settings;
                SaveSettings();

                if (_settings.ShowWidget && _widgetWindow == null)
                {
                    ShowWidgetWindow();
                }
                else if (!_settings.ShowWidget && _widgetWindow != null)
                {
                    _widgetWindow.Close();
                    _widgetWindow = null;
                }
            }
        }

        private void ShowWidgetWindow()
        {
            if (_widgetWindow == null)
            {
                _widgetWindow = new WidgetWindow(_deadlines);
                _widgetWindow.Closed += (s, e) => _widgetWindow = null;
                _widgetWindow.DeadlineDeleted += (s, e) => SaveDeadlines();
                _widgetWindow.Show();
            }
            else
            {
                if (!_widgetWindow.IsVisible)
                {
                    _widgetWindow.Show();
                }
                _widgetWindow.Activate();
            }
        }

        private void UpdateWidget()
        {
            _widgetWindow?.UpdateDeadlines(_deadlines);
            UpdateTrayTooltip();
        }

        private void UpdateTrayTooltip()
        {
            var next = _deadlines.Where(d => !d.IsCompleted).OrderBy(d => d.DueDate).FirstOrDefault();
            _notifyIcon.Text = next != null
                ? $"DueTick - Next: {next.Title} at {next.DueDate}"
                : "DueTick - No upcoming deadlines";
        }

        private void SaveDeadlines() => _storage.SaveDeadlines(_deadlines);
        private void SaveSettings() => _storage.SaveSettings(_settings);

        protected override void OnExit(ExitEventArgs e)
        {
            _notifyIcon?.Dispose();
            _clipboardMonitor?.Dispose();
            _timerService?.Dispose();
            _widgetWindow?.Close();
            base.OnExit(e);
        }
    }
}
