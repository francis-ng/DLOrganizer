using DLOrganizer.Commands;
using DLOrganizer.ConfigProvider;
using DLOrganizer.Model;
using DLOrganizer.Utils;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using Windows.Storage.Pickers;

namespace DLOrganizer.ViewModels
{
    public class ConfigViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

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
                    NotifyPropertyChanged(nameof(SelectedConfig));
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

        public ConfigViewModel()
        {
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
                NotifyPropertyChanged(nameof(SelectedConfig));
            }
            SaveConfigs();
        }

        private async void BrowseOrDelete(bool configsSelected)
        {
            // Browse
            if (!AnyConfigsSelected)
            {
                var folderPicker = new FolderPicker();
                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle((Application.Current as App).Window);
                WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, hwnd);

                folderPicker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
                folderPicker.FileTypeFilter.Add("*");
                var folder = await folderPicker.PickSingleFolderAsync();
                if (folder != null)
                {
                    Destination = folder.Path;
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
            SaveConfigs();
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
                await ConfigManager.SaveConfigs(new List<Config>(ListConfigs), SettingsManager.Settings.ConfigFile).ConfigureAwait(true);
            }
            catch (IOException)
            {
                ContentDialog dialog = new ContentDialog();

                string warnText = Application.Current.Resources["WarnSaveConfigFailed"] as string;
                string caption = Application.Current.Resources["CaptionConfigSaveError"] as string;

                dialog.Title = caption;
                dialog.Content = warnText;
                dialog.PrimaryButtonText = Application.Current.Resources["Yes"] as string;
                dialog.CloseButtonText = Application.Current.Resources["No"] as string;
                dialog.IsSecondaryButtonEnabled = false;
                dialog.DefaultButton = ContentDialogButton.Primary;
                var result = await dialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    SaveConfigs();
                }
            }
        }

        public async void LoadConfigs()
        {
            try
            {
                ListConfigs = new ObservableCollection<Config>(ConfigManager.Configs);
            }
            catch (InvalidOperationException)
            {
                ContentDialog dialog = new ContentDialog();

                string warnText = Application.Current.Resources["WarnLoadConfigFailed"] as string;
                string caption = Application.Current.Resources["CaptionConfigLoadError"] as string;

                dialog.Title = caption;
                dialog.Content = warnText;
                dialog.PrimaryButtonText = Application.Current.Resources["OK"] as string;
                dialog.CloseButtonText = Application.Current.Resources["Cancel"] as string;
                dialog.IsSecondaryButtonEnabled = false;
                dialog.DefaultButton = ContentDialogButton.Primary;
                var result = await dialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    ListConfigs = new ObservableCollection<Config>();
                }
                else
                {
                    Application.Current.Exit();
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
