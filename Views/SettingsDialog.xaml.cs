using System.Windows;
using System.Windows.Input;
using DueTick.Models;

namespace DueTick.Views
{
    public partial class SettingsDialog : Window
    {
        public Settings Settings { get; private set; }

        public SettingsDialog(Settings settings)
        {
            InitializeComponent();
            Settings = settings;
            LoadSettings();
        }

        private void LoadSettings()
        {
            AutoStartCheckBox.IsChecked = Settings.AutoStart;
            ClipboardCheckBox.IsChecked = Settings.EnableClipboardMonitoring;
            WidgetCheckBox.IsChecked = Settings.ShowWidget;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var newAutoStart = AutoStartCheckBox.IsChecked ?? false;
            var oldAutoStart = Settings.AutoStart;

            Settings.AutoStart = newAutoStart;
            Settings.EnableClipboardMonitoring = ClipboardCheckBox.IsChecked ?? false;
            Settings.ShowWidget = WidgetCheckBox.IsChecked ?? false;

            // Handle auto-start
            if (newAutoStart && !oldAutoStart)
            {
                DueTick.Services.AutoStartService.EnableAutoStart();
            }
            else if (!newAutoStart && oldAutoStart)
            {
                DueTick.Services.AutoStartService.DisableAutoStart();
            }

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
