using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.ObjectModel;
using dSearch.Helpers;
using static dSearch.Helpers.FileSearchHelpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;



namespace dSearch
{
    
    public class ApplicationSettings
    {
        public static ApplicationSettings AppSet { get; set; }
        
        public ObservableCollection<Drive> IndexedDrives { get; set; }

        // Get the application configuration file.
        public static System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        public static string jsonDrivesFromConfig = "";

        static ApplicationSettings()
        {
            AppSet = new ApplicationSettings();
            config = ConfigurationManager.OpenMappedExeConfiguration(getConfigFileMap(), ConfigurationUserLevel.None);

           

            if (config.AppSettings.Settings["jsonDrives"].Value == "")
            {
                var dr = GetLogicalDrives();

                config.AppSettings.Settings["jsonDrives"].Value = JsonConvert.SerializeObject(new
                {
                    indexedDrivesConf = dr
                });                
            }

            jsonDrivesFromConfig = config.AppSettings.Settings["jsonDrives"].Value;

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
            string drs = config.AppSettings.Settings["jsonDrives"].Value;
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

        public bool saveAppSettings()
        {       
            // Create a new configuration file by saving 
            // the application configuration to a new file.            

            string appPath = System.IO.Directory.GetCurrentDirectory();
            string configFile = System.IO.Path.Combine(appPath, "App.config");

            try
            {
                config.SaveAs(configFile, ConfigurationSaveMode.Full);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                config.Save();
            }            

            // Map the new configuration file.
            ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
            configFileMap.ExeConfigFilename = configFile;

            config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);

            

            string jsonDrives = JsonConvert.SerializeObject(new
            {
                indexedDrivesConf = IndexedDrives
            });


            config.AppSettings.Settings["jsonDrives"].Value = jsonDrives;
            

            // Save the configuration file.
            config.Save(ConfigurationSaveMode.Modified);

            // Force a reload of the changed section. This 
            // makes the new values available for reading.
            ConfigurationManager.RefreshSection("appSettings");

            

            if (jsonDrivesFromConfig != jsonDrives)
            {
                return true;
            }
            return false;
        }

        public static ExeConfigurationFileMap getConfigFileMap()
        {
            string appPath = System.IO.Directory.GetCurrentDirectory();
            string configFile = System.IO.Path.Combine(appPath, "App.config");

            // Map the new configuration file.
            ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
            configFileMap.ExeConfigFilename = configFile;

            if(ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None).HasFile == false)
            {
                config.SaveAs(configFile, ConfigurationSaveMode.Full);
            }

            return configFileMap;

        }
    }

       

}
