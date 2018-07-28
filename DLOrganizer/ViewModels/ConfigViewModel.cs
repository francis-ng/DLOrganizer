using DLOrganizer.Commands;
using DLOrganizer.ConfigProvider;
using DLOrganizer.Model;
using DLOrganizer.Properties;
using FolderSelect;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace DLOrganizer.ViewModels
{
    public class ConfigViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string configFile;
        private string _configName, _extension, _destination, _browseDeleteString, _addUpdateString;
        private ActionCommand<bool> browseDeleteCommand;
        private ActionCommand<bool> addUpdateCommand;
        private ActionCommand<bool> newConfigCommand;
        private Config selectedConfig;
        private bool nameFocused;

        #region Properties
        public ObservableCollection<Config> List_Configs
        {
            get; set;
        }

        public string BrowseDeleteButtonContent
        {
            get
            {
                return _browseDeleteString;
            }
            set
            {
                _browseDeleteString = value;
                NotifyPropertyChanged("BrowseDeleteButtonContent");
            }
        }

        public string AddUpdateButtonContent
        {
            get
            {
                return _addUpdateString;
            }
            set
            {
                _addUpdateString = value;
                NotifyPropertyChanged("AddUpdateButtonContent");
            }
        }

        public string ConfigName
        {
            get
            {
                return _configName;
            }
            set
            {
                _configName = value;
                NotifyPropertyChanged("ConfigName");
            }
        }

        public string Extension
        {
            get
            {
                return _extension;
            }
            set
            {
                _extension = value;
                NotifyPropertyChanged("Extension");
            }
        }

        public string Destination
        {
            get
            {
                return _destination;
            }
            set
            {
                _destination = value;
                NotifyPropertyChanged("Destination");
            }
        }

        public bool AnyConfigsSelected
        {
            get
            {
                return SelectedConfig != null;
            }
        }

        public bool NameIsFocused
        {
            get
            {
                return nameFocused;
            }
            set
            {
                nameFocused = value;
                NotifyPropertyChanged("NameIsFocused");
            }
        }

        public Config SelectedConfig
        {
            get
            {
                return selectedConfig;
            }
            set
            {
                if (selectedConfig != value)
                {
                    selectedConfig = value;
                    ConfigSelectionChanged();
                }
                NotifyPropertyChanged("SelectedConfig");
            }
        }

        public ICommand BrowseDeleteCommand
        {
            get
            {
                if (browseDeleteCommand == null)
                {
                    browseDeleteCommand = new ActionCommand<bool>(this.BrowseOrDelete, this.CanPerformCommand);
                }
                return browseDeleteCommand;
            }
        }

        public ICommand AddUpdateCommand
        {
            get
            {
                if (addUpdateCommand == null)
                {
                    addUpdateCommand = new ActionCommand<bool>(this.AddOrUpdate, this.CanPerformCommand);
                }
                return addUpdateCommand;
            }
        }

        public ICommand NewConfigCommand
        {
            get
            {
                if (newConfigCommand == null)
                {
                    newConfigCommand = new ActionCommand<bool>(this.NewConfig, this.CanPerformCommand);
                }
                return newConfigCommand;
            }
        }
        #endregion

        public ConfigViewModel(string configFile)
        {
            this.configFile = configFile;
            SetBrowseAddMode();
            LoadConfigs();
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region Commands
        private void AddOrUpdate(bool configsSelected)
        {
            // Add
            if (!AnyConfigsSelected)
            {
                Config config = new Config(ConfigName, Extension, Destination);
                List_Configs.Add(config);
                ClearConfigText();
                RefocusNameField();
            }
            // Update
            else
            {
                SelectedConfig.Name = ConfigName;
                SelectedConfig.Ext = Extension;
                SelectedConfig.Destination = Destination;
                ClearConfigSelection();
                ClearConfigText();
                RefocusNameField();
            }
        }

        private void BrowseOrDelete(bool configsSelected)
        {
            // Browse
            if (!AnyConfigsSelected)
            {
                FolderSelectDialog fldrDialog = new FolderSelectDialog();
                fldrDialog.InitialDirectory = Settings.Default.DefaultSource;
                fldrDialog.ShowDialog();
                if (fldrDialog.FileName != "")
                {
                    Destination = fldrDialog.FileName;
                }
            }
            else
            // Delete
            {
                int index = List_Configs.IndexOf(SelectedConfig);
                if (List_Configs.Count - 1 == index)
                {
                    index--;
                }
                List_Configs.Remove(SelectedConfig);
                if (index > 0)
                {
                    SelectedConfig = List_Configs[index];
                }
            }
        }

        private void NewConfig(bool dummy)
        {
            ClearConfigSelection();
            ClearConfigText();
            SetBrowseAddMode();
            RefocusNameField();
        }
        #endregion

        #region Helper Functions
        public void SaveConfigs()
        {
            try
            {
                ConfigManager.SaveConfigs(new List<Config>(List_Configs), configFile);
            }
            catch (IOException ex)
            {
                string warnText = @"Error when saving configuration file. Your current configuration has not been saved. Click Yes to try again, or No to continue without saving.";
                string caption = "Configuration Save Error";
                MessageBoxButton buttons = MessageBoxButton.YesNo;
                MessageBoxImage icon = MessageBoxImage.Warning;
                MessageBoxResult result = MessageBox.Show(warnText, caption, buttons, icon);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        SaveConfigs();
                        break;
                    case MessageBoxResult.No:
                        break;
                }
            }
        }

        public void LoadConfigs()
        {
            try
            {
                ConfigManager.LoadConfigs(configFile);
                List_Configs = new ObservableCollection<Config>(ConfigManager.Configs);
            }
            catch (InvalidOperationException ex)
            {
                string warnText = @"Error reading configuration file. Click OK to start with a fresh file, or Cancel to exit.";
                string caption = "Configuration Load Error";
                MessageBoxButton buttons = MessageBoxButton.OKCancel;
                MessageBoxImage icon = MessageBoxImage.Warning;
                MessageBoxResult result = MessageBox.Show(warnText, caption, buttons, icon);
                switch (result)
                {
                    case MessageBoxResult.OK:
                        List_Configs = new ObservableCollection<Config>();
                        break;
                    case MessageBoxResult.Cancel:
                        Application.Current.Shutdown();
                        break;
                }
            }
        }

        private void ClearConfigSelection()
        {
            SelectedConfig = null;
        }

        private void SetBrowseAddMode()
        {
            BrowseDeleteButtonContent = @"Browse";
            AddUpdateButtonContent = @"Add";
        }

        private void SetUpdateDeleteMode()
        {
            BrowseDeleteButtonContent = @"Delete";
            AddUpdateButtonContent = @"Update";
        }

        private void ConfigSelectionChanged()
        {
            if (null == SelectedConfig)
            {
                ClearConfigText();
                SetBrowseAddMode();
            }
            else
            {
                ConfigName = SelectedConfig.Name;
                Extension = SelectedConfig.Ext;
                Destination = SelectedConfig.Destination;
                RefocusNameField();
                SetUpdateDeleteMode();
            }
        }

        private void RefocusNameField()
        {
            NameIsFocused = false;
            NameIsFocused = true;
        }

        private void ClearConfigText()
        {
            ConfigName = "";
            Extension = "";
            Destination = "";
        }
        #endregion

        #region Command Validations
        private bool IsConfigSelected()
        {
            return AnyConfigsSelected;
        }

        private bool CanPerformCommand()
        {
            return true;
        }
        #endregion
    }
}
