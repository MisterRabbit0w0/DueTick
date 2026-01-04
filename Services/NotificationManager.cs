using System;
using System.Windows;
using DueTick.Models;

namespace DueTick.Services
{
    public static class NotificationManager
    {
        private static System.Windows.Forms.NotifyIcon _notifyIcon;

        public static void Initialize(System.Windows.Forms.NotifyIcon notifyIcon)
        {
            _notifyIcon = notifyIcon;
        }

        public static void ShowNotification(Deadline deadline)
        {
            var timeLeft = deadline.DueDate - DateTime.Now;
            var timeStr = timeLeft.TotalHours < 1
                ? $"{(int)timeLeft.TotalMinutes} minutes"
                : timeLeft.TotalHours < 24
                    ? $"{(int)timeLeft.TotalHours} hours"
                    : $"{(int)timeLeft.TotalDays} days";

            if (_notifyIcon != null)
            {
                _notifyIcon.ShowBalloonTip(5000, "DueTick Reminder", 
                    $"{deadline.Title}\nDue in {timeStr} at {deadline.DueDate:g}", 
                    System.Windows.Forms.ToolTipIcon.Info);
            }
            else
            {
                MessageBox.Show($"Deadline Reminder: {deadline.Title}\nDue in {timeStr} at {deadline.DueDate:g}", "DueTick Reminder");
            }
        }
    }
}
