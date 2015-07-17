using PTK.WPF;
using iNGen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro.Controls.Dialogs;
using GalaSoft.MvvmLight.CommandWpf;

namespace iNGen.ViewModels
{
    public class HomeViewModel: ViewModelBase
    {
        public ServerModelSet ServerModelSet {get; set;}
        public bool AttemptReconnectOnServerConnectionFail {get; set;}
        public string ApplicationLog { get; set; }
        public Server SelectedServer { get; set; }

        public RelayCommand DeleteSelectedServerCommand { get; set; }
        public RelayCommand NewServerCommand { get; set; }
        public event EventHandler ApplicationLogUpdated;

        public HomeViewModel()
        {
            ServerModelSet = App.ModelManager.Get<ServerModelSet>();
            ApplicationLog = "";

            #region Connection Related

            App.ArkRcon.ServerConnectionStarting += ArkRcon_ServerConnectionStarting;
            App.ArkRcon.ServerConnectionSucceeded += ArkRcon_ServerConnectionSucceeded;
            App.ArkRcon.ServerConnectionDisconnected += ArkRcon_ServerConnectionDisconnected;
            App.ArkRcon.ServerConnectionFailed += ArkRcon_ServerConnectionFailed;
            App.ArkRcon.ServerConnectionDropped += ArkRcon_ServerConnectionDropped;
            App.ArkRcon.ServerAuthSucceeded += (s, args) => WriteToApplicationLog(args.Message, args.Timestamp);
            App.ArkRcon.ServerAuthFailed += (s, args) => WriteToApplicationLog(args.Message, args.Timestamp);

            #endregion

            Messenger.Default.Register<NotificationMessage<MessageDialogResult>>(this, OnDialogResult);
            
            DeleteSelectedServerCommand = new RelayCommand(DeleteServer);
            NewServerCommand = new RelayCommand(NewServer);
        }

        #region Connection Methods

        void ArkRcon_ServerConnectionDropped(object sender, Ark.ServerConnectionEventArgs e)
        {
            ((Server)e.ConnectionInfo).IsConnected = false;
            WriteToApplicationLog(e.Message, e.Timestamp);

            if (App.ModelManager.Get<UserSettings>().GeneralSettings.IsAutoReconnectEnabled)
            {
                InitiateReconnect(e.ConnectionInfo as Server);
                AttemptReconnectOnServerConnectionFail = true;
            }
        }

        void ArkRcon_ServerConnectionFailed(object sender, Ark.ServerConnectionEventArgs e)
        {
            ((Server)e.ConnectionInfo).IsConnected = false;
            WriteToApplicationLog(e.Message, e.Timestamp);

            if (AttemptReconnectOnServerConnectionFail && App.ModelManager.Get<UserSettings>().GeneralSettings.IsAutoReconnectEnabled)
                InitiateReconnect(e.ConnectionInfo as Server);
        }

        void ArkRcon_ServerConnectionDisconnected(object sender, Ark.ServerConnectionEventArgs e)
        {
            ((Server)e.ConnectionInfo).IsConnected = false;
            WriteToApplicationLog(e.Message, e.Timestamp);
        }

        void ArkRcon_ServerConnectionSucceeded(object sender, Ark.ServerConnectionEventArgs e)
        {
            WriteToApplicationLog(e.Message, e.Timestamp);

            if (App.ModelManager.Get<UserSettings>().GeneralSettings.DoReconnectEveryFiveMinutes)
            {
                WriteToApplicationLog("Auto Reconnect Every Five minutes is Enabled", e.Timestamp);
                ForceReconnect(e.ConnectionInfo as Server);
            }
        }

        void ArkRcon_ServerConnectionStarting(object sender, Ark.ServerConnectionEventArgs e)
        {
            ((Server)e.ConnectionInfo).IsConnected = true;
            WriteToApplicationLog(e.Message, e.Timestamp);
        }

        #endregion

        private void DeleteServer()
        {
            //if (SelectedServer == null || SelectedServer.IsConnected)
            //    return;
            Messenger.Default.Send(new NotificationMessage<string>(string.Format("Confirm Deleting Server: {0}", SelectedServer.DisplayName), "ShowConfirmDeleteServerDialog"));
        }


        private void OnDialogResult(NotificationMessage<MessageDialogResult> result)
        {
            switch (result.Notification)
            {
                case "ConfirmDeleteServerDialogResult":
                    if (SelectedServer == null) return;
                    if (result.Content == MessageDialogResult.Negative) return;
                    var server = SelectedServer;
                    SelectedServer = null;
                    ServerModelSet.DeleteServer(server);
                    break;
                default:
                    return;
            }
        }

        private void NewServer()
        {
            ServerModelSet.NewServer();
            SelectedServer = ServerModelSet.Servers.Last();
        }



        public override void Cleanup()
        {
            base.Cleanup();
            Messenger.Default.Unregister<NotificationMessage<MessageDialogResult>>(this);
        }

        private void WriteToApplicationLog(string message, DateTime timestamp)
        {
            ApplicationLog += System.Environment.NewLine + timestamp.ToString("(hh:mm tt) ") + message;
            if(ApplicationLogUpdated != null)
                ApplicationLogUpdated(this, null);
        }

        private void InitiateReconnect(Server server)
        {
            WriteToApplicationLog("Auto-Reconnecting in 10 seconds...", DateTime.Now);
            if (!App.ArkRcon.IsConnected)
            {
                NormalReconnect(server);
            }
        }

        private async void ForceReconnect(Server server)
        {
            await Task.Delay(5 * 1000 * 60);
            await App.ArkRcon.Connect(server);
        }

        private async void NormalReconnect(Server server)
        {
            await Task.Delay(10 * 1000);
            await App.ArkRcon.Connect(server);
        }
    }
}
