using DLOrganizer.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace DLOrganizer.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public sealed partial class MainView : Page
    {
        delegate void textUpdater(string text);

        public MainViewModel viewmodel { get; set; }

        public MainView()
        {
            InitializeComponent();
            viewmodel = new MainViewModel();
        }
    }
}
