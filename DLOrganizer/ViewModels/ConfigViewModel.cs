using DLOrganizer.Commands;
using DLOrganizer.ConfigProvider;
using DLOrganizer.Model;
using DLOrganizer.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Forms;
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
        public ObservableCollection<Config> ListConfigs { get; set; }

        public string BrowseDeleteButtonContent
        {
            get
            {
                return _browseDeleteString;
            }
            set
            {
                _browseDeleteString = value;
                NotifyPropertyChanged(nameof(BrowseDeleteButtonContent));
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
                NotifyPropertyChanged(nameof(AddUpdateButtonContent));
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
                NotifyPropertyChanged(nameof(ConfigName));
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
                NotifyPropertyChanged(nameof(Extension));
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
                NotifyPropertyChanged(nameof(Destination));
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
                NotifyPropertyChanged(nameof(NameIsFocused));
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
                NotifyPropertyChanged(nameof(SelectedConfig));
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
                var config = new Config(ConfigName, Extension, Destination);
                ListConfigs.Add(config);
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
                using (var fldrDialog = new FolderBrowserDialog())
                {
                    fldrDialog.RootFolder = Environment.SpecialFolder.MyComputer;
                    if (string.IsNullOrWhiteSpace(Destination))
                    {
                        fldrDialog.SelectedPath = Settings.Default.DefaultSource;
                    }
                    else
                    {
                        fldrDialog.SelectedPath = Destination;
                    }
                    if (fldrDialog.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(fldrDialog.SelectedPath))
                    {
                        Destination = fldrDialog.SelectedPath;
                    }
                }
            }
            else
            // Delete
            {
                int index = ListConfigs.IndexOf(SelectedConfig);
                if (ListConfigs.Count - 1 == index)
                {
                    index--;
                }
                ListConfigs.Remove(SelectedConfig);
                if (index > 0)
                {
                    SelectedConfig = ListConfigs[index];
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
        public async void SaveConfigs()
        {
            try
            {
                await ConfigManager.SaveConfigs(new List<Config>(ListConfigs), configFile).ConfigureAwait(true);
            }
            catch (IOException)
            {
                string warnText = Strings.WarnSaveConfigFailed;
                string caption = Strings.CaptionConfigSaveError;
                MessageBoxButton buttons = MessageBoxButton.YesNo;
                MessageBoxImage icon = MessageBoxImage.Warning;
                MessageBoxResult result = System.Windows.MessageBox.Show(warnText, caption, buttons, icon);
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
                ListConfigs = new ObservableCollection<Config>(ConfigManager.Configs);
            }
            catch (InvalidOperationException)
            {
                string warnText = Strings.WarnLoadConfigFailed;
                string caption = Strings.CaptionConfigLoadError;
                MessageBoxButton buttons = MessageBoxButton.OKCancel;
                MessageBoxImage icon = MessageBoxImage.Warning;
                MessageBoxResult result = System.Windows.MessageBox.Show(warnText, caption, buttons, icon);
                switch (result)
                {
                    case MessageBoxResult.OK:
                        ListConfigs = new ObservableCollection<Config>();
                        break;
                    case MessageBoxResult.Cancel:
                        System.Windows.Application.Current.Shutdown();
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
