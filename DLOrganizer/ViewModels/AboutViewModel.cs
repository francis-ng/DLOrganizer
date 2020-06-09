using DLOrganizer.Properties;
using System.ComponentModel;

namespace DLOrganizer.ViewModels
{
    public class AboutViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string version;

        public string Version
        {
            get
            {
                return version;
            }
            set
            {
                version = value;
                NotifyPropertyChanged(nameof(Version));
            }
        }

        public AboutViewModel(string version)
        {
            Version = Strings.AppTitle + " " + version;
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
