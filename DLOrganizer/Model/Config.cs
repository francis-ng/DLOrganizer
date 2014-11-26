using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLOrganizer.Model
{
    public class Config
    {
        private string _name;
        private string _ext;
        private string _dest;

        public Config() { }

        public Config(string name, string ext, string dest)
        {
            _name = name;
            _ext = ext;
            _dest = dest;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Ext
        {
            get { return _ext; }
            set { _ext = value; }
        }

        public string Destination {
            get { return _dest; }
            set { _dest = value; }
        }

        public override string ToString()
        {
            return string.Format("Name: {0}, Ext: {1}, Dest: {2}", Name, Ext, Destination);
        }
    }
}
