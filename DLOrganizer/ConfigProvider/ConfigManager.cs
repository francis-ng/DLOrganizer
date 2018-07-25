using DLOrganizer.Model;
using System.Collections.Generic;

namespace DLOrganizer.ConfigProvider
{
    public class ConfigManager
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
            configs = new List<Config>(new ConfigReader(configFile).getConfigs());
        }

        public static void SaveConfigs(List<Config> configs, string configFile)
        {
            ConfigManager.configs = configs;
            ConfigWriter writer = new ConfigWriter(configFile, Configs);
        }
    }
}
