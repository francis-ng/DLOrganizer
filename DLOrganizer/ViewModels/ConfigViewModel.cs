using DLOrganizer.Commands;
using DLOrganizer.ConfigProvider;
using DLOrganizer.Model;
using DLOrganizer.Properties;
using FolderSelect;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace DLOrganizer.ViewModels
{
    public class ConfigViewModel
    {
        private string configFile;
        private ActionCommand<bool> browseDeleteCommand;
        private ActionCommand<bool> addUpdateCommand;
        private ActionCommand<bool> newConfigCommand;
        private int selectedConfig;

        #region Properties
        public ObservableCollection<Config> List_Configs
        {
            get; set;
        }

        public string BrowseDeleteButtonContent
        {
            get; set;
        }

        public string AddUpdateButtonContent
        {
            get; set;
        }

        public string ConfigName
        {
            get; set;
        }

        public string Extension
        {
            get; set;
        }

        public string Destination
        {
            get; set;
        }

        public bool AnyConfigsSelected
        {
            get
            {
                return SelectedConfig >= 0;
            }
        }

        public int SelectedConfig
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

        #region Commands
        private void AddOrUpdate(bool configsSelected)
        {
            if (!AnyConfigsSelected)
            {
                Config config = new Config(ConfigName, Extension, Destination);
                List_Configs.Add(config);
                ClearConfigText();
                //Focus name field
            }
            else
            {
                Config config = List_Configs[SelectedConfig];
                config.Name = ConfigName;
                config.Ext = Extension;
                config.Destination = Destination;
                SelectedConfig = -1;
                ClearConfigText();
                //Focus name field
            }
        }

        private void BrowseOrDelete(bool configsSelected)
        {
            if (!AnyConfigsSelected)
            {
                FolderSelectDialog fldrDialog = new FolderSelectDialog();
                fldrDialog.InitialDirectory = Settings.Default.DefaultSource;
                fldrDialog.ShowDialog();
                if (fldrDialog.FileName != "")
                {
                    Destination = fldrDialog.FileName;
                    // Focus AddUpdate button
                }
            }
            else
            {
                List_Configs.RemoveAt(SelectedConfig);
                if (List_Configs.Count == SelectedConfig)
                {
                    SelectedConfig--;
                }
                else if (List_Configs.Count > 0)
                {
                    SelectedConfig = SelectedConfig;
                }
            }
        }

        private void NewConfig(bool dummy)
        {
            //Unselect all
            ClearConfigText();
            //Focus name field
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
            if (SelectedConfig == -1)
            {
                ClearConfigText();
                SetBrowseAddMode();
            }
            else
            {
                Config config = List_Configs[SelectedConfig];
                ConfigName = config.Name;
                Extension = config.Ext;
                Destination = config.Destination;
                //Focus name field
                SetUpdateDeleteMode();
            }
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
