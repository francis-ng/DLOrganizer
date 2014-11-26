using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DLOrganizer;
using DLOrganizer.Model;
using DLOrganizer.ConfigProvider;

namespace UnitTest
{
    [TestClass]
    public class XmlReadWriteTest
    {
        [TestMethod]
        public void XmlWriteTest()
        {
            List<Config> configs = new List<Config>();
            configs.Add(new Config("", "mkv", @"D:\Anime"));
            configs.Add(new Config("Nishikino Maki", "mkv", @"D:\Anime"));
            configs.Add(new Config("iDOLM@STER", "", @"D:\Anime"));
            configs.Add(new Config("Bullet Girls", "mp4", @"D:\Anime"));
            configs.Add(new Config("KanColle", "mkv", @"D:\Anime"));

            ConfigWriter writer = new ConfigWriter("testconfig.xml", configs);
            Assert.IsTrue(File.Exists("testconfig.xml"));
        }

        [TestMethod]
        public void XmlReadTest()
        {
            ConfigReader reader = new ConfigReader("testconfig.xml");
            List<Config> configs = reader.getConfigs();
            foreach (Config config in configs)
            {
                Console.WriteLine(config.ToString());
            }
            Assert.IsNotNull(reader.getConfigs());
        }
    }
}
