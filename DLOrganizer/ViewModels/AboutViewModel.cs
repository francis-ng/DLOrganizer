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
                NotifyPropertyChanged("Version");
            }
        }

        public AboutViewModel(string version)
        {
            Version = @"DLOrganizer " + version;
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
