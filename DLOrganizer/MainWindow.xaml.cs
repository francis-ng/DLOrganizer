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
        private ObservableCollection<Config> configs;

        delegate void textUpdater(string text);

        public MainWindow()
        {
            InitializeComponent();
            Initialize();
            LoadConfigs();
        }

        private void Initialize()
        {
            Closing += OnWindowClosing;
            txt_srcFolder.Text = Settings.Default.DefaultSource;
            txt_srcFolder.KeyUp += ProcessSubmit;
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
            Process();
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
                int curIndex = lstb_configs.SelectedIndex;
                configs.RemoveAt(lstb_configs.SelectedIndex);
                lstb_configs.Items.Refresh();
                if (lstb_configs.Items.Count == curIndex)
                {
                    lstb_configs.SelectedIndex = curIndex - 1;
                }
                else if (lstb_configs.HasItems)
                {
                    lstb_configs.SelectedIndex = curIndex;
                }
            }
        }

        private void btn_addupdate_Click(object sender, RoutedEventArgs e)
        {
            AddOrUpdate();
        }

        private void updateText(string line)
        {
            if (txt_log.Dispatcher.CheckAccess())
            {
                txt_log.Text += line;
                if (txt_log.LineCount > 50)
                {
                    int length = txt_log.GetLineLength(0);
                    txt_log.Text = txt_log.Text.Substring(length, txt_log.Text.Length - length);
                }
                txt_log.ScrollToEnd();
            }
            else
            {
                txt_log.Dispatcher.BeginInvoke(new textUpdater(updateText), new string[] { line });
            }
        }

        private void btn_newconfig_Click(object sender, RoutedEventArgs e)
        {
            lstb_configs.UnselectAll();
            clearConfigText();
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
                clearConfigText();
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

        private void AddOrUpdate()
        {
            string name = txt_configName.Text;
            string ext = txt_configExt.Text;
            string dest = txt_configDest.Text;
            if (lstb_configs.SelectedIndex == -1)
            {
                Config config = new Config(name, ext, dest);
                configs.Add(config);
                clearConfigText();
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
                lstb_configs.UnselectAll();
                clearConfigText();
                txt_configName.Focus();
            }
        }

        private void Process()
        {
            bool shouldSimulate = (bool)chkbx_simulate.IsChecked;
            int selected = cmb_sanitize.SelectedIndex;
            try
            {
                FileProcessor fp = new FileProcessor(configs, txt_srcFolder.Text);
                fp.LogChanged += new EventHandler<LogEventArgs>(logUpdated);
                Thread oThread = new Thread(() => fp.processFiles(shouldSimulate, selected));
                oThread.Start();
            }
            catch (Exception ex)
            {
                updateText(ex.Message);
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

        private void clearConfigText()
        {
            txt_configName.Text = "";
            txt_configExt.Text = "";
            txt_configDest.Text = "";
        }

        private void logUpdated(object sender, LogEventArgs e)
        {
            TextBox logBox = txt_log;
            updateText(e.LogMessage + "\n");
        }
    }
}
