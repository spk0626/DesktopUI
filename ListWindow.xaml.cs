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
using System.Windows.Shapes;

namespace DesktopUI
{
    public partial class ListWindow : Window
    {
        public ListWindow()
        {
        }

        public ListWindow(ListVM vm)
        {
            DataContext = vm;
            InitializeComponent();
            vm.CloseAction = () => Close();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }
    }
}
