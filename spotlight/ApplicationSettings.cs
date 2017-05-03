﻿using System;
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
using Newtonsoft.Json.Linq;
using System.Reflection;

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

            if (ConfigurationManager.AppSettings["jsonDrives"] == "")
            {
                var dr = GetLogicalDrives();
                
                ConfigurationManager.AppSettings["jsonDrives"] = JsonConvert.SerializeObject(new
                {
                    indexedDrivesConf = dr
                });                
            }

            getDrivesFromSettings();
            AppSet.IndexedDrives.CollectionChanged += IndexedDrives_CollectionChanged;
        }

        private static void IndexedDrives_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            
        }

        public static void getApplicationSettings()
        {
            getDrivesFromSettings();
        }

        public static void getDrivesFromSettings()
        {
            string drs = ConfigurationManager.AppSettings["jsonDrives"];
            JArray indDr = JsonConvert.DeserializeObject<dynamic>(drs).indexedDrivesConf;
       
            AppSet.IndexedDrives = indDr.ToObject<ObservableCollection<Drive>>();
        }

        public static ApplicationSettings GetAppSet()
        {
            return AppSet;
        }

        public void updateAppSet(ApplicationSettings app)
        {
            AppSet = app;
        }

        public void saveAppSettings()
        {
            //ConfigurationManager.AppSettings["jsonDrives"] = JsonConvert.SerializeObject(new
            //{
            //    indexedDrivesConf = IndexedDrives
            //});

            string appPath = System.IO.Directory.GetCurrentDirectory();
            string configFile = System.IO.Path.Combine(appPath, "App.config");
            ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
            configFileMap.ExeConfigFilename = configFile;

            Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);

            configuration.AppSettings.Settings["jsonDrives"].Value = JsonConvert.SerializeObject(new
            {
                indexedDrivesConf = IndexedDrives
            });

            configuration.Save();
            ConfigurationManager.RefreshSection("appSettings");
        }

        
    }

       

}
