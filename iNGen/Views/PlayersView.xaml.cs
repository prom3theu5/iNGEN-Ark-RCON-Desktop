using Ark.Models;
using iNGen.ViewModels;
using SteamWeb;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace iNGen.Views
{
    public partial class PlayersView : UserControl
    {
        public PlayersViewModel ViewModel { get { return DataContext as PlayersViewModel; } }

        public PlayersView()
        {
            InitializeComponent();
        }
    }
}
