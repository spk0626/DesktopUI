using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace DesktopUI
{
    public partial class MainWindowVM : ObservableObject
    {
        public ICommand EnterSystemCommand { get; }

        public MainWindowVM()
        {
            EnterSystemCommand = new RelayCommand(EnterSystem);
            
        }


        private void EnterSystem()
        {

            var vm = new ListVM();
            ListWindow window = new ListWindow(vm);
            App.Current.MainWindow.Hide();
            window.Closed += (s, e) =>
            {
                Application.Current.MainWindow.ShowDialog();
            };

            window.ShowDialog();
        }

    }
}
