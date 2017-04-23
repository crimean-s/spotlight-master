using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static spotlight.Helpers.FileSearchHelpers;

namespace spotlight
{
    public static class ApplicationSettings
    {
        public static List<string> indexedDrives { get; private set; }

        public static void updateApplicationSettings()
        {
            if (ConfigurationManager.AppSettings["indexedDrives"] == "")
            {
                ConfigurationManager.AppSettings["indexedDrives"] = string.Join(";", GetLogicalDrives().ToArray());
            }
        }        
    }


}
