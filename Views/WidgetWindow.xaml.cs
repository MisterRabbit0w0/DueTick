using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using DueTick.Models;

namespace DueTick.Views
{
    public partial class WidgetWindow : Window
    {
        public event EventHandler DeadlineDeleted;

        private List<Deadline> _allDeadlines;

        public WidgetWindow(List<Deadline> deadlines)
        {
            InitializeComponent();
            _allDeadlines = deadlines;
            UpdateDeadlines(deadlines);
            PositionWindow();
            Closing += WidgetWindow_Closing;
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
                this.DragMove();
        }

        private void PositionWindow()
        {
            var screen = Screen.PrimaryScreen.WorkingArea;
            Left = (screen.Width - Width) / 2;
            Top = (screen.Height - Height) / 2; // Center screen
        }

        public void UpdateDeadlines(List<Deadline> deadlines)
        {
            _allDeadlines = deadlines;
            var upcoming = deadlines.Where(d => !d.IsCompleted)
                                   .OrderBy(d => d.DueDate)
                                   .ToList();
            DeadlinesListBox.ItemsSource = upcoming;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as System.Windows.Controls.Button;
            var deadline = button?.DataContext as Deadline;
            if (deadline != null)
            {
                var dialog = new ConfirmDeleteDialog($"Delete deadline '{deadline.Title}'?");
                dialog.Owner = this;
                if (dialog.ShowDialog() == true)
                {
                    _allDeadlines.Remove(deadline);
                    DeadlineDeleted?.Invoke(this, EventArgs.Empty);
                    // Save will be handled by the caller, but for now update UI
                    UpdateDeadlines(_allDeadlines);
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void WidgetWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Cancel the close and hide the window instead
            e.Cancel = true;
            Hide();
        }
    }
}
