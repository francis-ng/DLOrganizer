using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace DLOrganizer.Utils
{
    public class MappingProvider
    {
        const string TITLEMAPPINGXML = "TitleMappings.xml";
        static Dictionary<string, string> _titleTrans;
        static List<string> _titleKeys;
        private static int[] _status;
        private static List<string> errorList;

        static MappingProvider()
        {
            _titleTrans = new Dictionary<string, string>();
            _titleKeys = new List<string>();
            errorList = new List<string>();
            loadMappings(TITLEMAPPINGXML);
        }

        private static void loadMappings(string path)
        {
            XmlReader reader = XmlReader.Create(path);
            while (reader.Read())
            {
                if (reader.IsStartElement())
                {
                    switch (reader.Name)
                    {
                        case "anime":
                            string title = reader["title"];
                            string folder = "";
                            if (reader.Read())
                            {
                                folder = reader.Value.Trim();
                                _titleTrans.Add(title, folder);
                                _titleKeys.Add(title);
                            }
                            break;
                    }
                }
            }
            reader.Close();
            _status = new int[_titleTrans.Count];
        }

        public static string GetFolder(string filename)
        {
            for (int i = 0; i < _titleKeys.Count; i++)
            {
                if (filename.Contains(_titleKeys[i]))
                {
                    _status[i] = 1;
                    return _titleTrans[_titleKeys[i]];
                }
            }
            return null;
        }

        public static string GetMappingErrors()
        {
            string output = "\nCould not find mappings for:\n";
            for (int i = 0; i < _status.Length; i++)
            {
                if (_status[i] != 1)
                {
                    output += _titleKeys[i] + "\n";
                }
            }
            return output;
        }
    }
}
