using System;
using System.Collections.Generic;

namespace DueTick.Models
{
    public class Settings
    {
        public List<TimeSpan> PresetOffsets { get; set; } = new List<TimeSpan>()
        {
            TimeSpan.FromDays(2),
            TimeSpan.FromHours(12),
            TimeSpan.FromHours(6),
            TimeSpan.FromDays(1),
            TimeSpan.FromHours(1),
            TimeSpan.FromMinutes(30)
        };

        public bool AutoStart { get; set; } = true;
        public bool EnableClipboardMonitoring { get; set; } = true;
        public bool ShowWidget { get; set; } = true;
    }
}
