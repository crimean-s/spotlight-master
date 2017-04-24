using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static spotlight.Helpers.FileSearchHelpers;
using System.Windows;
using System.Collections.ObjectModel;

namespace spotlight
{
    public class ApplicationSettings
    {
        public static ApplicationSettings AppSet { get; set; }

        public ObservableCollection<string> IndexedDrives { get; private set; }

        

        static ApplicationSettings()
        {
            AppSet = new ApplicationSettings();

            if (ConfigurationManager.AppSettings["indexedDrives"] == "")
            {
                ConfigurationManager.AppSettings["indexedDrives"] = string.Join(";", GetLogicalDrives().ToArray());
            }
        } 

        public static void getApplicationSettings()
        {
            getDrivesFromSettings();
        }

        public static void getDrivesFromSettings()
        {
            AppSet.IndexedDrives = new ObservableCollection<string>(ConfigurationManager.AppSettings["indexedDrives"].Split(new char[] { ';' }));
        }

        public static ApplicationSettings GetAppSet()
        {
            return AppSet;
        }

        public void updateAppSet(ApplicationSettings app)
        {
            AppSet = app;
        }
    }


}
