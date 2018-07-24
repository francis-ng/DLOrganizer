using DLOrganizer.Model;
using DLOrganizer.Properties;
using FolderSelect;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DLOrganizer.Views
{
    /// <summary>
    /// Interaction logic for ConfigView.xaml
    /// </summary>
    public partial class ConfigView : UserControl
    {
        public ConfigView()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            txt_configName.KeyUp += SubmitAddOrUpdate;
            txt_configExt.KeyUp += SubmitAddOrUpdate;
            txt_configDest.KeyUp += SubmitAddOrUpdate;
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
                //configs.RemoveAt(lstb_configs.SelectedIndex);
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
            //AddOrUpdate();
        }

        private void btn_newconfig_Click(object sender, RoutedEventArgs e)
        {
            lstb_configs.UnselectAll();
            //clearConfigText();
            txt_configName.Focus();
        }

        private void lstb_configs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = lstb_configs.SelectedIndex;
            if (index == -1)
            {
                //clearConfigText();
                btn_addupdate.Content = "Add";
                btn_browsedelete.Content = "Browse";
            }
            else
            {
                /*Config config = configs[index];
                txt_configName.Text = config.Name;
                txt_configExt.Text = config.Ext;
                txt_configDest.Text = config.Destination;
                txt_configName.Focus();
                btn_addupdate.Content = "Update";
                btn_browsedelete.Content = "Delete";*/
            }
        }

        private void SubmitAddOrUpdate(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //AddOrUpdate();
            }
        }

        

        
    }
}
