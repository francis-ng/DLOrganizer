using DLOrganizer.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using static DLOrganizer.Model.LogEvent;

namespace DLOrganizer
{
    class FileProcessor
    {
        private string _activeDir;
        private List<string> _fileList;
        private List<Config> _configs;

        public event EventHandler<LogEvent> LogChanged;

        public FileProcessor(List<Config> configs, string srcDir)
        {
            _configs = configs;
            _activeDir = srcDir;
            _fileList = new List<string>();
            GetFileList();
        }

        protected virtual void LogAdded(LogEvent e)
        {
            LogChanged?.Invoke(this, e);
        }

        public List<DLOJob> GenerateJobList(bool simulate, int sanitize)
        {
            SanitizeFilenames(sanitize, simulate);
            List<DLOJob> jobs = new List<DLOJob>();
            foreach (Config config in _configs)
            {
                List<string> files = new List<string>();
                if (string.IsNullOrEmpty(config.Ext))
                {
                    files.AddRange(_fileList.Where(s => s.Contains(config.Name, StringComparison.OrdinalIgnoreCase)).ToList());
                }
                else if (string.IsNullOrEmpty(config.Name))
                {
                    files.AddRange(_fileList.Where(s => s.EndsWith(config.Ext, StringComparison.OrdinalIgnoreCase)).ToList());
                }
                else
                {
                    files.AddRange(_fileList.Where(s => s.Contains(config.Name, StringComparison.OrdinalIgnoreCase) && s.EndsWith(config.Ext, StringComparison.OrdinalIgnoreCase)).ToList());
                }
                foreach (string file in files)
                {
                    jobs.Add(new DLOJob { Source = file , Destination = config.Destination });
                }
            }
            return jobs;
        }

        public void ProcessFiles(List<DLOJob> jobs, bool simulate)
        {
            foreach (DLOJob job in jobs)
            {
                ProcessFile(job.Source, job.Destination, simulate);
            }
        }

        private void ProcessFile(string file, string dest, bool simulate)
        {
            if (dest != null)
            {
                if (!Directory.Exists(dest))
                {
                    if (!simulate) Directory.CreateDirectory(dest);
                    LogAdded(
                        new LogEvent
                        {
                            Type = EventType.FolderCreate,
                            Destination = dest,
                            Success = true
                        }
                    );
                }
                dest = Path.Combine(dest, Path.GetFileName(file));
                if (simulate) Thread.Sleep(500);
                else File.Move(file, dest); 
                LogAdded(
                    new LogEvent
                    {
                        Type = EventType.FileMove,
                        Source = file,
                        Destination = dest,
                        Success = true
                    }
                );
            }
        }

        private void GetFileList()
        {
            _fileList = Directory.GetFiles(_activeDir).ToList();
        }

        public void SanitizeFilenames(int type, bool simulate)
        {
            for (int i = 0; i < _fileList.Count; i++)
            {
                string fileName = Path.GetFileName(_fileList[i]);
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
                if (!newName.Equals(fileName, StringComparison.OrdinalIgnoreCase))
                {
                    if (!simulate) File.Move(_fileList[i], newName);
                    _fileList[i] = Path.GetDirectoryName(_fileList[i]) + @"\" + newName;
                    LogAdded(
                        new LogEvent
                        {
                            Type = EventType.FileRename,
                            Source = fileName,
                            Destination = newName,
                            Success = true
                        }
                    );
                }
            }
        }
    }
}
