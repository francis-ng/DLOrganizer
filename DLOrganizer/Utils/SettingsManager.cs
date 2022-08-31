using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DLOrganizer.Utils
{
    public static class SettingsManager
    {
        private static Settings _settings;
        public static Settings Settings { 
            get
            {
                if (_settings == null)
                {
                    _settings = new Settings();
                }
                return _settings;
            }
        }

        public static void LoadSettings()
        {
            if (File.Exists(Settings.SettingsFile)) {
                var jsonString = File.ReadAllText(Settings.SettingsFile);
                _settings = JsonSerializer.Deserialize<Settings>(jsonString);
            }
        }

        public static async Task SaveSettings()
        {
            using (FileStream fs = File.Create(Settings.SettingsFile))
            {
                await JsonSerializer.SerializeAsync(fs, Settings).ConfigureAwait(false);
            }
        }
    }

    public class Settings
    {
        public string DefaultSource { get; set; }
        public string ConfigFile { get; set; }
        public string SettingsFile { get; set; }
    }
}
