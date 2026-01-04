using System;
using System.IO;
using System.Reflection;

namespace DueTick.Services
{
    public static class AutoStartService
    {
        private static readonly string StartupPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
        private static readonly string AppName = "DueTick";
        private static readonly string ShortcutPath = Path.Combine(StartupPath, $"{AppName}.lnk");

        public static bool IsAutoStartEnabled()
        {
            return File.Exists(ShortcutPath);
        }

        public static void EnableAutoStart()
        {
            if (IsAutoStartEnabled()) return;

            // Create a shortcut to the exe
            var exePath = Assembly.GetExecutingAssembly().Location;

            // For simplicity, create a batch file instead of shortcut (since shortcut requires COM)
            var batchPath = Path.Combine(StartupPath, $"{AppName}.bat");
            File.WriteAllText(batchPath, $"start \"\" \"{exePath}\"");
        }

        public static void DisableAutoStart()
        {
            if (File.Exists(ShortcutPath))
            {
                File.Delete(ShortcutPath);
            }

            var batchPath = Path.Combine(StartupPath, $"{AppName}.bat");
            if (File.Exists(batchPath))
            {
                File.Delete(batchPath);
            }
        }
    }
}
