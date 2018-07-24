using DLOrganizer.ConfigProvider;
using DLOrganizer.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace DLOrganizer.ViewModels
{
    public class ConfigViewModel
    {
        private string configFile;

        public ObservableCollection<Config> List_Configs
        {
            get; set;
        }

        public string TxtBox_FileName
        {
            get; set;
        }

        public string TxtBox_Extension
        {
            get; set;
        }

        public string TxtBox_Destination
        {
            get; set;
        }

        public ConfigViewModel(string configFile)
        {
            this.configFile = configFile;
        }

        public void SaveConfigs()
        {
            try
            {
                List<Config> config = new List<Config>(List_Configs);
                ConfigWriter writer = new ConfigWriter(configFile, config);
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
                List_Configs = new ObservableCollection<Config>(new ConfigReader(configFile).getConfigs());
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

        private void AddOrUpdate()
        {/*
            if (lstb_configs.SelectedIndex == -1)
            {
                Config config = new Config(TxtBox_FileName, TxtBox_Extension, TxtBox_Destination);
                //configs.Add(config);
                ClearConfigText();
                txt_configName.Focus();
            }
            else
            {
                int index = lstb_configs.SelectedIndex;
                Config config = List_Configs[index];
                config.Name = name;
                config.Ext = ext;
                config.Destination = dest;
                lstb_configs.Items.Refresh();
                lstb_configs.UnselectAll();
                ClearConfigText();
                txt_configName.Focus();
            }*/
        }

        private void ClearConfigText()
        {
            TxtBox_FileName = "";
            TxtBox_Extension = "";
            TxtBox_Destination = "";
        }
    }
}
