using DLOrganizer.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

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
            if (File.Exists(configFile))
            {
                var serializer = new JsonSerializer();
                try
                {
                    using (StreamReader sr = new StreamReader(configFile))
                    {
                        using (JsonReader reader = new JsonTextReader(sr))
                        {
                            configs = serializer.Deserialize<List<Config>>(reader);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public static void SaveConfigs(List<Config> configs, string configFile)
        {
            ConfigManager.configs = configs;

            var serializer = new JsonSerializer();
            try
            {
                using (StreamWriter sw = new StreamWriter(configFile))
                {
                    using (JsonWriter writer = new JsonTextWriter(sw))
                    {
                        serializer.Serialize(writer, Configs, typeof(List<Config>));
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
