using DLOrganizer.Model;
using System.Text.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DLOrganizer.ConfigProvider
{
    public static class ConfigManager
    {
        private static List<Config> configs;

        public static List<Config> Configs
        {
            get
            {
                if (configs == null)
                {
                    configs = new List<Config>();
                }
                return configs;
            }
        }

        public static void LoadConfigs(string configFile)
        {
            if (File.Exists(configFile))
            {
                var jsonString = File.ReadAllText(configFile);
                configs = JsonSerializer.Deserialize<List<Config>>(jsonString);
            }
        }

        public static async Task SaveConfigs(List<Config> configs, string configFile)
        {
            ConfigManager.configs = configs;
            using (FileStream fs = File.Create(configFile))
            {
                await JsonSerializer.SerializeAsync(fs, Configs).ConfigureAwait(false);
            }
        }
    }
}
