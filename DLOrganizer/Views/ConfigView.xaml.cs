using DLOrganizer.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace DLOrganizer.Views
{
    /// <summary>
    /// Interaction logic for ConfigView.xaml
    /// </summary>
    public partial class ConfigView : Page
    {
        public ConfigViewModel viewmodel { get; set; }

        public ConfigView()
        {
            InitializeComponent();
            viewmodel = new ConfigViewModel();
        }
    }
}
