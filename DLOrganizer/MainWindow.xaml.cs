﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DLOrganizer.Properties;
using DLOrganizer.Utils;
using DLOrganizer.Model;
using DLOrganizer.ConfigProvider;
using FolderSelect;
using System.IO;

namespace DLOrganizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string configFile = "config.xml";
        private ObservableCollection<Config> configs;

        public MainWindow()
        {
            InitializeComponent();
            Closing += OnWindowClosing;
            txt_srcFolder.Text = Settings.Default.DefaultSource;
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

        private void btn_browse_Click(object sender, RoutedEventArgs e)
        {
            FolderSelectDialog fldrDialog = new FolderSelectDialog();
            fldrDialog.InitialDirectory = Settings.Default.DefaultSource;
            fldrDialog.ShowDialog();
            if (fldrDialog.FileName != "")
            {
                txt_srcFolder.Text = fldrDialog.FileName;
                btn_process.Focus();
            }
        }

        private void btn_process_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FileProcessor fp = new FileProcessor(configs, txt_srcFolder.Text);
                fp.processFiles((bool)chkbx_simulate.IsChecked, cmb_sanitize.SelectedIndex);
                updateText(fp.getLogs());
                //updateText("Errors:\n" + MappingProvider.GetMappingErrors());
            }
            catch (Exception ex)
            {
                updateText(ex.Message);
            }
        }

        private void btn_browsedelete_Click(object sender, RoutedEventArgs e)
        {
            if (lstb_configs.SelectedIndex == -1)
            {
                FolderSelectDialog fldrDialog = new FolderSelectDialog();
                fldrDialog.InitialDirectory = Settings.Default.DefaultSource;
                fldrDialog.ShowDialog();
                if (fldrDialog.FileName != "")
                {
                    txt_configDest.Text = fldrDialog.FileName;
                    btn_addupdate.Focus();
                }
            }
            else
            {
                configs.RemoveAt(lstb_configs.SelectedIndex);
                lstb_configs.Items.Refresh();
            }
        }

        private void btn_addupdate_Click(object sender, RoutedEventArgs e)
        {
            string name = txt_configName.Text;
            string ext = txt_configExt.Text;
            string dest = txt_configDest.Text;
            if (lstb_configs.SelectedIndex == -1)
            {
                Config config = new Config(name, ext, dest);
                configs.Add(config);
                txt_configName.Text = "";
                txt_configExt.Text = "";
                txt_configDest.Text = "";
                txt_configName.Focus();
            }
            else
            {
                int index = lstb_configs.SelectedIndex;
                Config config = configs[index];
                config.Name = name;
                config.Ext = ext;
                config.Destination = dest;
                lstb_configs.Items.Refresh();
                txt_configName.Text = "";
                txt_configExt.Text = "";
                txt_configDest.Text = "";
                txt_configName.Focus();
            }
        }

        private void updateText(string line)
        {
            txt_log.Text += line;
            if (txt_log.LineCount > 50)
            {
                int length = txt_log.GetLineLength(0);
                txt_log.Text = txt_log.Text.Substring(length, txt_log.Text.Length - length);
            }
            txt_log.ScrollToEnd();
        }

        private void btn_newconfig_Click(object sender, RoutedEventArgs e)
        {
            lstb_configs.UnselectAll();
            txt_configName.Text = "";
            txt_configExt.Text = "";
            txt_configDest.Text = "";
            txt_configName.Focus();
        }

        private void btn_clearlog_Click(object sender, RoutedEventArgs e)
        {
            txt_log.Clear();
        }

        private void lstb_configs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = lstb_configs.SelectedIndex;
            if (index == -1)
            {
                txt_configName.Text = "";
                txt_configExt.Text = "";
                txt_configDest.Text = "";
                btn_addupdate.Content = "Add";
                btn_browsedelete.Content = "Browse";
            }
            else
            {
                Config config = configs[index];
                txt_configName.Text = config.Name;
                txt_configExt.Text = config.Ext;
                txt_configDest.Text = config.Destination;
                txt_configName.Focus();
                btn_addupdate.Content = "Update";
                btn_browsedelete.Content = "Delete";
            }
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