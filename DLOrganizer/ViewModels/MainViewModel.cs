using DLOrganizer.Model;
using DLOrganizer.Properties;
using DLOrganizer.Utils;
using FolderSelect;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;

namespace DLOrganizer.ViewModels
{
    public partial class MainViewModel
    {
        public string SourceFolder
        {
            get; set;
        }

        public string LogContents
        {
            get; set;
        }

        public bool Simulate
        {
            get; set;
        }

        // Sanitize
    }
}
