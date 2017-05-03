using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;

namespace spotlight.Views
{    
    public partial class MySettings : MetroWindow
    {
        public MySettings()
        {
            InitializeComponent();
            this.Loaded += MySettings_Loaded;
        }

        private void MySettings_Loaded(object sender, RoutedEventArgs e)
        {
            listDrives.ItemsSource = ApplicationSettings.AppSet.IndexedDrives;
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            ApplicationSettings.AppSet.saveAppSettings();
        }
    }
}
