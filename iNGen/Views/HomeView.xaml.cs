using GalaSoft.MvvmLight.Messaging;
using iNGen.Models;
using iNGen.ViewModels;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
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
    public partial class HomeView : UserControl
    {
        public HomeViewModel ViewModel { get { return DataContext as HomeViewModel; } }

        public HomeView()
        {
            InitializeComponent();
            ViewModel.ApplicationLogUpdated += (s, args) => ApplicationLogScrollViewer.ScrollToBottom();
        }

        private void ServersListView_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Delete && ServersListView.HasItems && ServersListView.SelectedItem != null)
            {
                if (ViewModel.SelectedServer == null || ViewModel.SelectedServer.IsConnected)
                    return;
                Messenger.Default.Send(new NotificationMessage<string>(string.Format("Confirm Deleting Server: {0}", ViewModel.SelectedServer.DisplayName), "ShowConfirmDeleteServerDialog"));
            }
        }

        private async void ConnectButtonClick(object sender, RoutedEventArgs e)
        {
            if(ViewModel.SelectedServer != null)
            {
                ViewModel.AttemptReconnectOnServerConnectionFail = false;
                await App.ArkRcon.Connect(ViewModel.SelectedServer);
            }
        }

        private async void ServerListViewItemDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ViewModel.SelectedServer != null)
            {
                ViewModel.AttemptReconnectOnServerConnectionFail = false;
                await App.ArkRcon.Connect(ViewModel.SelectedServer);
            }
        }

        private void DisconnectButtonClick(object sender, RoutedEventArgs e)
        {
            ViewModel.AttemptReconnectOnServerConnectionFail = false;
            App.ArkRcon.Disconnect();
        }
    }
}
