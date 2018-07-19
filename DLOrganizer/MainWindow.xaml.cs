using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DLOrganizer.Properties;
using DLOrganizer.Utils;
using DLOrganizer.Model;
using DLOrganizer.ConfigProvider;
using DLOrganizer.ViewModels;
using FolderSelect;
using System.IO;

namespace DLOrganizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private const string VERSION = "v1.0.4";
        private const string configFile = "config.xml";
        public ObservableCollection<Config> Configs
        {
            get; set;
        }

        public MainWindow()
        {
            InitializeComponent();
            Initialize();
            LoadConfigs();
        }

        private void Initialize()
        {
            Closing += OnWindowClosing;
            txt_configName.KeyUp += SubmitAddOrUpdate;
            txt_configExt.KeyUp += SubmitAddOrUpdate;
            txt_configDest.KeyUp += SubmitAddOrUpdate;
            lbl_versionInfo.Content = lbl_versionInfo.Content.ToString() + VERSION;
        }

        private void LoadConfigs()
        {
            try
            {
                configs = new ObservableCollection<Config>(new ConfigReader(configFile).getConfigs());
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
                        configs = new ObservableCollection<Config>();
                        break;
                    case MessageBoxResult.Cancel:
                        Application.Current.Shutdown();
                        break;
                }
            }
            lstb_configs.DataContext = configs;
        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            saveConfigs();
            Settings.Default.DefaultSource = txt_srcFolder.Text;
            Settings.Default.Save();
        }
        

        

        private void btn_clearlog_Click(object sender, RoutedEventArgs e)
        {
            txt_log.Clear();
        }

        

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tc_tabs.SelectedIndex != 1)
            {
                if (configs != null)
                {
                    saveConfigs();
                }
            }
            //DataContext = new MainViewModel();
        }

        private void SubmitAddOrUpdate(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AddOrUpdate();
            }
        }

        private void ProcessSubmit(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Process();
            }
        }

        private void saveConfigs()
        {
            try
            {
                List<Config> config = new List<Config>(configs);
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
                        saveConfigs();
                        break;
                    case MessageBoxResult.No:
                        break;
                }
            }
        }

        

        
    }
}
