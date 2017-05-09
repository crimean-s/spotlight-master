using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;

namespace dSearch
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private TaskbarIcon notifyIcon;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //create the notifyicon (it's a resource declared in NotifyIconResources.xaml
            notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");
        }

        /// <summary>
        /// Удаление иконки при выходе
        /// </summary>
        protected override void OnExit(ExitEventArgs e)
        {
            notifyIcon.Dispose(); 
            base.OnExit(e);
        }
    }
}
