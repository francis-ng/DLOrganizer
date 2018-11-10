using DLOrganizer.Properties;
using DLOrganizer.Utils;
using FolderSelect;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
