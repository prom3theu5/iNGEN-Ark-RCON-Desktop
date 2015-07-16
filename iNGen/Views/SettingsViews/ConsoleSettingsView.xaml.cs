using iNGen.ViewModels.SettingsViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace iNGen.Views.SettingsViews
{
    public partial class ConsoleSettingsView : UserControl
    {
        public ConsoleSettingsViewModel ViewModel {get; set;}

        public ConsoleSettingsView()
        {
            ViewModel = new ConsoleSettingsViewModel();
            InitializeComponent();
        }

        private void OpenLogDirectory_Click(object sender, RoutedEventArgs e)
        {
            String exePath = System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName;
            string dir = System.IO.Path.GetDirectoryName(exePath);
            Process.Start(new ProcessStartInfo(System.IO.Path.Combine(dir, "Logs")));
        }
    }
}
