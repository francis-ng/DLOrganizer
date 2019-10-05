using DLOrganizer.Commands;
using DLOrganizer.ConfigProvider;
using DLOrganizer.Model;
using DLOrganizer.Properties;
using DLOrganizer.Utils;
using FolderSelect;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Input;

namespace DLOrganizer.ViewModels
{
    public partial class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ActionCommand<bool> processCommand;
        private ActionCommand<bool> browseCommand;
        private ActionCommand<bool> clearLogCommand;
        private string sourceFolder, logContents;

        #region Properties
        public string SourceFolder
        {
            get
            {
                return sourceFolder;
            }
            set
            {
                sourceFolder = value;
                NotifyPropertyChanged("SourceFolder");
            }
        }

        public string LogContents
        {
            get
            {
                return logContents;
            }
            set
            {
                logContents = value;
                NotifyPropertyChanged("LogContents");
            }
        }

        public bool Simulate
        {
            get; set;
        }

        public int SelectedSanitize
        {
            get; set;
        }

        public ICommand ProcessCommand
        {
            get
            {
                if (processCommand == null)
                {
                    processCommand = new ActionCommand<bool>(this.Process, this.CanProcess);
                }
                return processCommand;
            }
        }

        public ICommand BrowseCommand
        {
            get
            {
                if (browseCommand == null)
                {
                    browseCommand = new ActionCommand<bool>(this.Browse, this.CanBrowse);
                }
                return browseCommand;
            }
        }

        public ICommand ClearLogCommand
        {
            get
            {
                if (clearLogCommand == null)
                {
                    clearLogCommand = new ActionCommand<bool>(this.ClearLog, this.CanClearLog);
                }
                return clearLogCommand;
            }
        }

        public ObservableCollection<SanitizeType> SanitizeTypes
        {
            get; private set;
        }
        #endregion

        // Sanitize

        public MainViewModel()
        {
            SourceFolder = Settings.Default.DefaultSource;
            SanitizeTypes = new ObservableCollection<SanitizeType>();
            SanitizeTypes.Add(new SanitizeType(@"Don't standardize filenames", 0));
            SanitizeTypes.Add(new SanitizeType(@"Change '_' to ' '", 0));
            SanitizeTypes.Add(new SanitizeType(@"Change ' ' to '_'", 0));
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region Commands
        private void Process(bool simulate)
        {
            try
            {
                var fp = new FileProcessor(ConfigManager.Configs, SourceFolder);
                fp.LogChanged += new EventHandler<LogEventArgs>(LogUpdated);
                var oThread = new Thread(() => fp.processFiles(Simulate, SelectedSanitize));
                oThread.Start();
            }
            catch (Exception ex)
            {
                AddToLog(ex.Message);
            }
        }

        private void Browse(bool dummy)
        {
            var fldrDialog = new FolderSelectDialog();
            if (string.IsNullOrWhiteSpace(SourceFolder))
            {
                fldrDialog.Path = Settings.Default.DefaultSource;
            }
            else
            {
                fldrDialog.Path = SourceFolder;
            }
            if (fldrDialog.ShowDialog() == DialogResult.OK && fldrDialog.Path != "")
            {
                SourceFolder = fldrDialog.Path;
            }
        }

        private void ClearLog(bool dummy)
        {
            LogContents = string.Empty;
        }
        #endregion

        #region Helper Functions
        private void AddToLog(string message)
        {
            LogContents += message + "\n";
        }

        private void LogUpdated(object sender, LogEventArgs e)
        {
            AddToLog(e.LogMessage);
        }
        #endregion

        #region Command Validations
        private bool CanProcess()
        {
            return true;
        }

        private bool CanBrowse()
        {
            return true;
        }

        private bool CanClearLog()
        {
            return true;
        }
        #endregion
    }
}
