using DLOrganizer.Properties;
using DLOrganizer.Utils;
using FolderSelect;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace DLOrganizer.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : UserControl
    {
        delegate void textUpdater(string text);

        public MainView()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            txt_srcFolder.Text = Settings.Default.DefaultSource;
            txt_srcFolder.KeyUp += ProcessSubmit;
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

        private void logUpdated(object sender, LogEventArgs e)
        {
            TextBox logBox = txt_log;
            updateText(e.LogMessage + "\n");
        }
    }
}
