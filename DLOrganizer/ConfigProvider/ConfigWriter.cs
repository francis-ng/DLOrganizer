using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DLOrganizer.Model;

namespace DLOrganizer.ConfigProvider
{
    public class ConfigWriter
    {
        private string _configFile;

        public ConfigWriter(string configFile) {
            _configFile = configFile;
        }

        public ConfigWriter(string configFile, List<Config> configs)
        {
            _configFile = configFile;
            saveConfigs(configs);
        }

        public void saveConfigs(List<Config> configs)
        {
            using (FileStream fs = File.Create(_configFile))
            {
                XmlSerializer formatter = new XmlSerializer(typeof(List<Config>));
                formatter.Serialize(fs, configs);
            }
        }
    }
}
