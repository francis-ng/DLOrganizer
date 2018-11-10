using DLOrganizer.Model;
using DLOrganizer.Utils;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DLOrganizer
{
    class FileProcessor
    {
        private string _activeDir;
        private List<string> _fileList;
        private List<Config> _configs;
        private static List<string> _logs;

        public event EventHandler<LogEventArgs> LogChanged;

        public FileProcessor(List<Config> configs, string srcDir)
        {
            _configs = configs;
            _activeDir = srcDir;
            _fileList = new List<string>();
            _logs = new List<string>();
        }

        protected virtual void LogAdded(LogEventArgs e)
        {
            LogChanged?.Invoke(this, e);
        }

        public void processFiles(bool simulate, int sanitize)
        {
            getFileList();
            sanitizeFilenames(sanitize, simulate);
            foreach (Config config in _configs) {
                List<string> files;
                if (config.Ext.Equals(""))
                {
                    files = _fileList.Where(s => s.Contains(config.Name)).ToList();
                }
                else if (config.Name.Equals(""))
                {
                    files = _fileList.Where(s => s.EndsWith(config.Ext)).ToList();
                }
                else
                {
                    files = _fileList.Where(s => s.Contains(config.Name) && s.EndsWith(config.Ext)).ToList();
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
                var args = new LogEventArgs();
                if (!Directory.Exists(dest))
                {
                    if (!simulate) Directory.CreateDirectory(dest);
                    args.LogMessage = "Created directory " + dest;
                    LogAdded(args);
                }
                dest = Path.Combine(dest, Path.GetFileName(file));
                if (!simulate) FileSystem.MoveFile(file, dest, UIOption.AllDialogs);
                args.LogMessage = "Moved " + file + " to " + dest + ".";
                LogAdded(args);
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
                    var args = new LogEventArgs();
                    args.LogMessage = log;
                    LogAdded(args);
                }
            }
        }
    }
}
