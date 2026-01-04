using System;

namespace DueTick.Models
{
    public class Deadline
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public TimeSpan ReminderOffset { get; set; } = TimeSpan.FromHours(1);
        public bool IsCompleted { get; set; } = false;

        public DateTime ReminderTime => DueDate - ReminderOffset;

        public bool ShouldNotify => !IsCompleted && DateTime.Now >= ReminderTime && DateTime.Now < DueDate;
    }
}
