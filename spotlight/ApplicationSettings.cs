using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.ObjectModel;
using spotlight.Helpers;
using static spotlight.Helpers.FileSearchHelpers;
using Newtonsoft.Json;

//http://piotrgankiewicz.com/2016/06/06/storing-c-app-settings-with-json/

namespace spotlight
{
    public class ApplicationSettings
    {
        public static ApplicationSettings AppSet { get; set; }

        public ObservableCollection<Drive> IndexedDrives { get; set; } 

        public string pole { get; set; } = "dlkcnsldkc";

        

        static ApplicationSettings()
        {
            AppSet = new ApplicationSettings();

            if (ConfigurationManager.AppSettings["indexedDrives"] == "")
            {
                var dr = GetLogicalDrives();
                foreach (var item in dr)
                {
                    ConfigurationManager.AppSettings["indexedDrives"] += item.Name + ";";
                }
                ConfigurationManager.AppSettings["jsonDrives"] = JsonConvert.SerializeObject(new
                {
                    operations = dr
                });
            }
        } 

        public static void getApplicationSettings()
        {
            getDrivesFromSettings();
        }

        public static void getDrivesFromSettings()
        {
            List<string> dr = new List<string>(ConfigurationManager.AppSettings["indexedDrives"].Split(new char[] { ';' }));
            foreach (var item in dr)
            {

            }

            string drs = ConfigurationManager.AppSettings["jsonDrives"];
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
