using DLOrganizer.Commands;
using DLOrganizer.Properties;
using DLOrganizer.ViewModels;
using System.Collections.Generic;
using System.Windows.Input;

namespace DLOrganizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private const string VERSION = "v1.0.4";
        private const string configFile = "config.xml";

        private MainViewModel mainViewModel;
        private ConfigViewModel configViewModel;
        private AboutViewModel aboutViewModel;

        private ButtonCommand<int> navCommand;

        public List<NavButton> MainButtons
        {
            get; set;
        }

        public ICommand NavigateCommand
        {
            get
            {
                if (navCommand == null)
                {
                    navCommand = new ButtonCommand<int>(this.ChangePage, this.CanNavigate);
                }
                return navCommand;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            Closing += OnWindowClosing;

            MainButtons = new List<NavButton>();
            MainButtons.Add(new NavButton("Process", NavigateCommand, 0));
            MainButtons.Add(new NavButton("Configuration", NavigateCommand, 1));
            MainButtons.Add(new NavButton("About", NavigateCommand, 2));
            MenuItems.ItemsSource = MainButtons;

            navCommand = new ButtonCommand<int>(ChangePage, CanNavigate);

            mainViewModel = new MainViewModel();
            configViewModel = new ConfigViewModel(configFile);
            aboutViewModel = new AboutViewModel(VERSION);

            configViewModel.LoadConfigs();
            ChangePage(0);
        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            configViewModel.SaveConfigs();
            Settings.Default.DefaultSource = mainViewModel.SourceFolder;
            Settings.Default.Save();
        }

        public void ChangePage(int page)
        {
            // save configs if not null
            switch (page)
            {
                case 0:
                    DataContext = mainViewModel;
                    break;
                case 1:
                    DataContext = configViewModel;
                    break;
                case 2:
                    DataContext = aboutViewModel;
                    break;
            }
        }

        public bool CanNavigate()
        {
            return true;
        }

        public class NavButton
        {
            public string Label
            {
                get;set;
            }

            public ICommand Command
            {
                get;set;
            }

            public int PageNumber
            {
                get;set;
            }

            public NavButton(string label, ICommand command, int page)
            {
                Label = label;
                Command = command;
                PageNumber = page;
            }
        }
    }
}
