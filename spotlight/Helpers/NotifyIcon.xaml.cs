using System;
using System.Windows;
using System.Windows.Input;

namespace dSearch
{
    public class NotifyIcon
    {
        /// <summary>
        /// Открывает окно из трея
        /// </summary>
        public ICommand ShowWindowCommand
        {
            get
            {
                return new DelegateCommand
                {                    
                    CommandAction = () => {
                        Application.Current.MainWindow.WindowState = WindowState.Normal;
                        Application.Current.MainWindow.Activate();
                        Application.Current.MainWindow.Topmost = true;
                        Application.Current.MainWindow.Topmost = false;
                        Application.Current.MainWindow.Focus();
                    }
                };
            }
        }

        /// <summary>
        /// Минимизирует окно
        /// </summary>
        public ICommand HideWindowCommand
        {
            get
            {
                return new DelegateCommand
                {                    
                    CommandAction = () => Application.Current.MainWindow.WindowState = WindowState.Minimized,
                    CanExecuteFunc = () => Application.Current.MainWindow != null
                };
            }
        }


        /// <summary>
        /// Закрывает приложение
        /// </summary>
        public ICommand ExitApplicationCommand
        {
            get
            {
                return new DelegateCommand { CommandAction = () => Application.Current.Shutdown() };
            }
        }
    }

    public class DelegateCommand : ICommand
    {
        public Action CommandAction { get; set; }
        public Func<bool> CanExecuteFunc { get; set; }

        public void Execute(object parameter)
        {
            CommandAction();
        }

        public bool CanExecute(object parameter)
        {
            return CanExecuteFunc == null || CanExecuteFunc();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
