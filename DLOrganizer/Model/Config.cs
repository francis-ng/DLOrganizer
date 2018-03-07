using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLOrganizer.Model
{
    public class Config
    {
        public Config() { }

        public Config(string name, string ext, string dest)
        {
            Name = name;
            Ext = ext;
            Destination = dest;
        }

        public string Name
        {
            get;
            set;
        }

        public string Ext
        {
            get;
            set;
        }

        public string Destination {
            get;
            set;
        }

        public override string ToString()
        {
            return string.Format("Name: {0}, Ext: {1}, Dest: {2}", Name, Ext, Destination);
        }
    }
}
