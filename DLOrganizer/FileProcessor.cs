using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;
using DLOrganizer.Utils;
using System.Collections.ObjectModel;
using DLOrganizer.Model;

namespace DLOrganizer
{
    class FileProcessor
    {
        private string _activeDir;
        private List<string> _fileList;
        private ObservableCollection<Config> _configs;
        private static List<string> _logs;

        public FileProcessor(ObservableCollection<Config> configs, string srcDir)
        {
            _configs = configs;
            _activeDir = srcDir;
            _fileList = new List<string>();
            _logs = new List<string>();
        }

        public void processFiles(bool simulate, int sanitize)
        {
            getFileList();
            sanitizeFilenames(sanitize, simulate);
            foreach (Config config in _configs) {
                List<string> files;
                if (config.Ext.Equals(""))
                {
                    files = _fileList.Where(s => s.Contains(config.Name)).ToList<string>();
                }
                else if (config.Name.Equals(""))
                {
                    files = _fileList.Where(s => s.EndsWith(config.Ext)).ToList<string>();
                }
                else
                {
                    files = _fileList.Where(s => s.Contains(config.Name) && s.EndsWith(config.Ext)).ToList<string>();
                }
                foreach (string file in files)
                {
                    processFile(file, config.Destination, simulate);
                }
            }
        }

        private void processFile(string file, string dest, bool simulate)
        {
            if (dest != null)
            {
                if (!Directory.Exists(dest))
                {
                    if (!simulate) Directory.CreateDirectory(dest);
                    _logs.Add("Created directory " + dest);
                }
                dest = Path.Combine(dest, Path.GetFileName(file));
                if (!simulate) FileSystem.MoveFile(file, dest, UIOption.AllDialogs);
                string log = "Moved " + file + " to " + dest + ".";
                _logs.Add(log);
            }
        }

        private void getFileList()
        {
            _fileList = Directory.GetFiles(_activeDir).ToList<string>();
        }

        public void sanitizeFilenames(int type, bool simulate)
        {
            for (int i = 0; i < _fileList.Count; i++)
            {
                string fileName = Path.GetFileName(_fileList[i]);
                string log = "Renamed " + fileName + " to ";
                string newName = fileName;
                switch (type)
                {
                    case 1:
                        newName = fileName.Replace('_', ' ');
                        break;
                    case 2:
                        newName = fileName.Replace(' ', '_');
                        break;
                }
                if (!newName.Equals(fileName))
                {
                    if (!simulate) FileSystem.RenameFile(_fileList[i], newName);
                    _fileList[i] = Path.GetDirectoryName(_fileList[i]) + @"\" + newName;
                    log += newName + ".";
                    _logs.Add(log);
                }
            }
        }

        public string getLogs()
        {
            string output = "";
            if (_logs.Count > 0)
            {
                foreach (string log in _logs)
                {
                    output += log + "\n";
                }
            }
            return output;
        }
    }
}
