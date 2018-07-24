using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLOrganizer.ViewModels
{
    public class AboutViewModel
    {
        public string Version
        {
            get; set;
        }

        public AboutViewModel(string version)
        {
            Version = version;
        }
    }
}
