using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DueTick.Models;

namespace DueTick.Views
{
    public class ReminderItem
    {
        public string Description { get; set; }
        public TimeSpan Offset { get; set; }
    }

    public partial class AddDeadlineDialog : Window
    {
        public Deadline Deadline { get; private set; }

        public AddDeadlineDialog(List<TimeSpan> presetOffsets)
        {
            InitializeComponent();
            PopulateReminderComboBox(presetOffsets);
            DueDatePicker.SelectedDate = DateTime.Now;
            DueTimeTextBox.Text = DateTime.Now.AddHours(2).ToString("HH:mm");
        }

        private void PopulateReminderComboBox(List<TimeSpan> presetOffsets)
        {
            if (presetOffsets == null || !presetOffsets.Any()) return;

            var items = presetOffsets.Distinct().Select(ts =>
            {
                var desc = ts.TotalDays >= 1
                    ? $"{(int)ts.TotalDays} day{(ts.TotalDays > 1 ? "s" : "")}"
                    : ts.TotalHours >= 1
                        ? $"{(int)ts.TotalHours} hour{(ts.TotalHours > 1 ? "s" : "")}"
                        : $"{(int)ts.TotalMinutes} minute{(ts.TotalMinutes > 1 ? "s" : "")}";
                return new ReminderItem { Description = desc, Offset = ts };
            }).ToList();

            ReminderComboBox.ItemsSource = items;
            ReminderComboBox.DisplayMemberPath = "Description";
            ReminderComboBox.SelectedIndex = 0;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
            {
                MessageBox.Show("Please enter a title.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!DueDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Please select a due date.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!TimeSpan.TryParse(DueTimeTextBox.Text, out var time))
            {
                MessageBox.Show("Please enter a valid time (HH:mm).", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var selectedItem = ReminderComboBox.SelectedItem as ReminderItem;
            if (selectedItem == null)
            {
                MessageBox.Show("Please select a reminder time.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var dueDate = DueDatePicker.SelectedDate.Value.Date.Add(time);

            // Validate that the due date is not in the past
            if (dueDate <= DateTime.Now)
            {
                MessageBox.Show("The due date and time must be in the future.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Deadline = new Deadline
            {
                Title = TitleTextBox.Text.Trim(),
                DueDate = dueDate,
                ReminderOffset = selectedItem.Offset
            };

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
