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
using MahApps.Metro.Controls.Dialogs;

namespace dSearch.Views
{

    public partial class MySettings : MetroWindow
    {
        private bool closeDialogSettings;

        public MySettings()
        {
            InitializeComponent();
            this.Loaded += MySettings_Loaded;
        }

        private void MySettings_Loaded(object sender, RoutedEventArgs e)
        {
            listDrives.ItemsSource = ApplicationSettings.AppSet.IndexedDrives;
            
        }
        

        private async void ShowConfirmationDialog(System.ComponentModel.CancelEventArgs e)
        {
            if (e.Cancel) return;

            // we want manage the closing itself!
            e.Cancel = !this.closeDialogSettings;
            // yes we want now really close the window
            if (this.closeDialogSettings) return;

            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Да, хочу",
                NegativeButtonText = "Нет",
                AnimateShow = true,
                AnimateHide = false
            };
            var result = await this.ShowMessageAsync(
                "Перезапустить приложение?",
                "Вы изменили настройки индексации. Хотите cделать рестарт приложения?",
                MessageDialogStyle.AffirmativeAndNegative, mySettings);

            this.closeDialogSettings = result == MessageDialogResult.Affirmative;

            if (this.closeDialogSettings)
            {
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ApplicationSettings.AppSet.saveAppSettings())
            {
                ShowConfirmationDialog(e);
            }                
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
