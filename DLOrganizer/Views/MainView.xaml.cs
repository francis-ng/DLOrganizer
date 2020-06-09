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
        }
    }
}
