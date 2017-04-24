using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static spotlight.Helpers.FileSearchHelpers;
using System.Windows;

namespace spotlight
{
    public class ApplicationSettings : DependencyObject
    {
        public ApplicationSettings appSet = new ApplicationSettings();



        public ApplicationSettings()
        {
            updateApplicationSettings();
        }

        public List<string> indexedDrives { get; private set; }

        public void updateApplicationSettings()
        {
            if (ConfigurationManager.AppSettings["indexedDrives"] == "")
            {
                ConfigurationManager.AppSettings["indexedDrives"] = string.Join(";", GetLogicalDrives().ToArray());
            }
        }        
    }


}
