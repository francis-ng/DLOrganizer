using System.ComponentModel;

namespace DLOrganizer.Model
{
    public class Config : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string name, ext, destination;

        public Config() { }

        public Config(string name, string ext, string dest)
        {
            Name = name;
            Ext = ext;
            Destination = dest;
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                NotifyPropertyChanged("Name");
            }
        }

        public string Ext
        {
            get
            {
                return ext;
            }
            set
            {
                ext = value;
                NotifyPropertyChanged("Ext");
            }
        }

        public string Destination
        {
            get
            {
                return destination;
            }
            set
            {
                destination = value;
                NotifyPropertyChanged("Destination");
            }
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return string.Format("Name: {0}, Ext: {1}, Dest: {2}", Name, Ext, Destination);
        }
    }
}
