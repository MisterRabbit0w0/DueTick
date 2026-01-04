using System;
using System.Text.RegularExpressions;
using Humanizer;

namespace DueTick.Utils
{
    public class DateParser
    {
        private static readonly Regex DateTimeRegex = new Regex(
            @"(?<date>\d{4}-\d{2}-\d{2}|\d{2}/\d{2}/\d{4}|\d{2}-\d{2}-\d{4})\s+(?<time>\d{1,2}:\d{2}(?:\s?[ap]m)?)",
            RegexOptions.IgnoreCase);

        private static readonly Regex TimeRegex = new Regex(
            @"(?<time>\d{1,2}:\d{2}(?:\s?[ap]m)?)",
            RegexOptions.IgnoreCase);

        public (string title, DateTime date)? Parse(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;

            // Try to parse structured date/time
            var match = DateTimeRegex.Match(text);
            if (match.Success)
            {
                var dateStr = match.Groups["date"].Value;
                var timeStr = match.Groups["time"].Value;

                if (DateTime.TryParse($"{dateStr} {timeStr}", out var date))
                {
                    var title = text.Replace(match.Value, "").Trim();
                    return (title.Length > 0 ? title : "Deadline", date);
                }
            }

            // Simple natural language: "tomorrow at 3pm", "in 2 days"
            try
            {
                var lower = text.ToLower();
                DateTime? result = null;

                if (lower.Contains("tomorrow"))
                {
                    result = DateTime.Now.AddDays(1);
                }
                else if (lower.Contains("today"))
                {
                    result = DateTime.Now.Date;
                }
                else if (Regex.IsMatch(lower, @"in (\d+) days?"))
                {
                    var daysMatch = Regex.Match(lower, @"in (\d+) days?");
                    if (int.TryParse(daysMatch.Groups[1].Value, out var days))
                    {
                        result = DateTime.Now.AddDays(days);
                    }
                }

                if (result.HasValue)
                {
                    // Try to find time
                    var timeMatch = TimeRegex.Match(text);
                    if (timeMatch.Success)
                    {
                        var timeStr = timeMatch.Groups["time"].Value;
                        if (TimeSpan.TryParse(timeStr, out var time))
                        {
                            result = result.Value.Date + time;
                        }
                    }

                    return ("Deadline", result.Value);
                }
            }
            catch
            {
                // Ignore parsing errors
            }

            return null;
        }
    }
}
