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
    public class ConfigReader
    {
        private List<Config> _configs;

        public ConfigReader(string configFile)
        {
            _configs = new List<Config>();
            if (File.Exists(configFile))
            {
                try
                {
                    using (FileStream fileStream = File.OpenRead(configFile))
                    {
                        MemoryStream memStream = new MemoryStream();
                        memStream.SetLength(fileStream.Length);
                        fileStream.Read(memStream.GetBuffer(), 0, (int)fileStream.Length);
                        XmlSerializer formatter = new XmlSerializer(typeof(List<Config>));
                        List<Config> curList = (List<Config>)formatter.Deserialize(memStream);
                        _configs.AddRange(curList);
                    }
                }
                catch (InvalidOperationException ex)
                {
                    throw new InvalidOperationException(@"Error reading config file.");
                }
            }
        }

        public List<Config> getConfigs()
        {
            return _configs;
        }
    }
}
