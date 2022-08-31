using DLOrganizer.ConfigProvider;
using DLOrganizer.Utils;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DLOrganizer
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            Title = Application.Current.Resources["AppTitle"] as string;
            content.Navigate(Type.GetType("DLOrganizer.Views.MainView"));
            ConfigManager.LoadConfigs(SettingsManager.Settings.ConfigFile);
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            var selectedItem = (NavigationViewItem)args.SelectedItem;
            string selectedItemTag = ((string)selectedItem.Tag);
            string pageName = "DLOrganizer.Views." + selectedItemTag;
            Type pageType = Type.GetType(pageName);
            content.Navigate(pageType);
        }
    }
}
