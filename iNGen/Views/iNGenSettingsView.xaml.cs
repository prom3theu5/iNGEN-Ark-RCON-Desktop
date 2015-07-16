using iNGen.ViewModels;
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

namespace iNGen.Views
{
    /// <summary>
    /// Interaction logic for iNGenSettingsView.xaml
    /// </summary>
    public partial class iNGenSettingsView : UserControl
    {
        public iNGenSettingsViewModel ViewModel {get; set;}

        public iNGenSettingsView()
        {
            ViewModel = new iNGenSettingsViewModel();
            InitializeComponent();
        }
    }
}
