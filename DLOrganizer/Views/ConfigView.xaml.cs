using DLOrganizer.Model;
using DLOrganizer.Properties;
using FolderSelect;
using System;
using System.Collections.Generic;
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

        private void btn_newconfig_Click(object sender, RoutedEventArgs e)
        {
            lstb_configs.UnselectAll();
            clearConfigText();
            txt_configName.Focus();
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

        private void clearConfigText()
        {
            txt_configName.Text = "";
            txt_configExt.Text = "";
            txt_configDest.Text = "";
        }
    }
}
