using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DueTick.Models;
using Newtonsoft.Json;

namespace DueTick.Services
{
    public class StorageService
    {
        private static readonly string AppDataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "DueTick");

        private static readonly string DeadlinesFile = Path.Combine(AppDataPath, "deadlines.json");
        private static readonly string SettingsFile = Path.Combine(AppDataPath, "settings.json");

        static StorageService()
        {
            Directory.CreateDirectory(AppDataPath);
        }

        public List<Deadline> LoadDeadlines()
        {
            if (!File.Exists(DeadlinesFile))
                return new List<Deadline>();

            var json = File.ReadAllText(DeadlinesFile);
            return JsonConvert.DeserializeObject<List<Deadline>>(json) ?? new List<Deadline>();
        }

        public void SaveDeadlines(List<Deadline> deadlines)
        {
            var json = JsonConvert.SerializeObject(deadlines, Formatting.Indented);
            File.WriteAllText(DeadlinesFile, json);
        }

        public Settings LoadSettings()
        {
            if (!File.Exists(SettingsFile))
                return new Settings();

            var json = File.ReadAllText(SettingsFile);
            return JsonConvert.DeserializeObject<Settings>(json) ?? new Settings();
        }

        public void SaveSettings(Settings settings)
        {
            var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(SettingsFile, json);
        }
    }
}
