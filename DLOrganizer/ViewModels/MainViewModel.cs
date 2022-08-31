using DLOrganizer.Commands;
using DLOrganizer.ConfigProvider;
using DLOrganizer.Model;
using DLOrganizer.Utils;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage.Pickers;

namespace DLOrganizer.ViewModels
{
    public partial class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ActionCommand<bool> processCommand;
        private ActionCommand<bool> browseCommand;
        private ActionCommand<bool> clearLogCommand;
        private string sourceFolder, logContents;
        private int totalFiles, filesCompleted;
        private DispatcherQueue dispatcher;

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
                SettingsManager.Settings.DefaultSource = value;
                NotifyPropertyChanged(nameof(SourceFolder));
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
                NotifyPropertyChanged(nameof(LogContents));
            }
        }

        public int TotalFiles
        {
            get
            {
                return totalFiles;
            }
            set
            {
                totalFiles = value;
                NotifyPropertyChanged(nameof(TotalFiles));
            }
        }

        public int FilesCompleted
        {
            get
            {
                return filesCompleted;
            }
            set
            {
                filesCompleted = value;
                NotifyPropertyChanged(nameof(FilesCompleted));
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
                    processCommand = new ActionCommand<bool>(Process, CanProcess);
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
                    browseCommand = new ActionCommand<bool>(Browse, CanBrowse);
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
                    clearLogCommand = new ActionCommand<bool>(ClearLog, CanClearLog);
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
            SourceFolder = SettingsManager.Settings.DefaultSource;
            SanitizeTypes = new ObservableCollection<SanitizeType>();
            SanitizeTypes.Add(new SanitizeType(@"Don't standardize filenames", 0));
            SanitizeTypes.Add(new SanitizeType(@"Change '_' to ' '", 1));
            SanitizeTypes.Add(new SanitizeType(@"Change ' ' to '_'", 2));
            TotalFiles = 0;
            FilesCompleted = 0;
            dispatcher = DispatcherQueue.GetForCurrentThread();
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
                var jobList = fp.GenerateJobList(Simulate, SelectedSanitize);
                TotalFiles = jobList.Count;
                fp.LogChanged += new EventHandler<LogEvent>(LogUpdated);
                Task.Run(() => fp.ProcessFiles(jobList, Simulate));
            }
            catch (Exception ex)
            {
                AddToLog(ex.Message);
                throw;
            }
        }

        private async void Browse(bool dummy)
        {
            var folderPicker = new FolderPicker();
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle((Application.Current as App).Window);
            WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, hwnd);

            folderPicker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            folderPicker.FileTypeFilter.Add("*");
            var folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                SourceFolder = folder.Path;
                SettingsManager.Settings.DefaultSource = folder.Path;
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

        private void LogUpdated(object sender, LogEvent e)
        {
            dispatcher.TryEnqueue(() =>
            {
                if (e.Type == LogEvent.EventType.FileMove && e.Success)
                {
                    FilesCompleted = filesCompleted + 1;
                }
                AddToLog(e.LogMessage);
            });
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
