using PTK.WPF;
using iNGen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iNGen.ViewModels
{
    public class HomeViewModel: Notifiable
    {
        public ServerModelSet ServerModelSet {get; set;}
        public bool AttemptReconnectOnServerConnectionFail {get; set;}
        public event EventHandler ApplicationLogUpdated;

        public HomeViewModel()
        {
            ServerModelSet = App.ModelManager.Get<ServerModelSet>();

            ApplicationLog = "";
            App.ArkRcon.ServerConnectionStarting += (s, args) =>
                {
                    ((Server)args.ConnectionInfo).IsConnected = true;
                    WriteToApplicationLog(args.Message, args.Timestamp);
                };
            App.ArkRcon.ServerConnectionSucceeded += (s, args) => 
            { 
                WriteToApplicationLog(args.Message, args.Timestamp); 

                if (App.ModelManager.Get<UserSettings>().GeneralSettings.DoReconnectEveryFiveMinutes)
                {
                    WriteToApplicationLog("Auto Reconnect Every Five minutes is Enabled", args.Timestamp); 
                    App.EventManager.Events.Add(new TimedEvent(5*1000*60, () =>
                    {
                        ForceReconnect(args.ConnectionInfo as Server);
                    }, true));
                }
            };
            App.ArkRcon.ServerConnectionDisconnected += (s, args) =>
                {
                    ((Server)args.ConnectionInfo).IsConnected = false;
                    WriteToApplicationLog(args.Message, args.Timestamp);
                };
            App.ArkRcon.ServerConnectionFailed += (s, args) =>
                {
                    ((Server)args.ConnectionInfo).IsConnected = false;
                    WriteToApplicationLog(args.Message, args.Timestamp);

                    if(AttemptReconnectOnServerConnectionFail && App.ModelManager.Get<UserSettings>().GeneralSettings.IsAutoReconnectEnabled)
                        InitiateReconnect(args.ConnectionInfo as Server);
                };
            App.ArkRcon.ServerConnectionDropped += (s, args) =>
                {
                    ((Server)args.ConnectionInfo).IsConnected = false;
                    WriteToApplicationLog(args.Message, args.Timestamp);

                    if(App.ModelManager.Get<UserSettings>().GeneralSettings.IsAutoReconnectEnabled)
                    {
                        InitiateReconnect(args.ConnectionInfo as Server);
                        AttemptReconnectOnServerConnectionFail = true;
                    }
                };
            App.ArkRcon.ServerAuthSucceeded += (s, args) => WriteToApplicationLog(args.Message, args.Timestamp);
            App.ArkRcon.ServerAuthFailed += (s, args) => WriteToApplicationLog(args.Message, args.Timestamp);
        }

        private string mApplicationLog;
        public string ApplicationLog
        {
            get { return mApplicationLog; }
            set { SetField(ref mApplicationLog, value); }
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
            App.EventManager.Events.Add(new TimedEvent(10000, () =>
                {
                    if(!App.ArkRcon.IsConnected)
                    {
                        App.ArkRcon.Connect(server);
                    }
                }, true));
        }

        private void ForceReconnect(Server server)
        {
            WriteToApplicationLog("Auto-Reconnecting in 3 seconds...", DateTime.Now);
            App.EventManager.Events.Add(new TimedEvent(3000, () =>
            {
               App.ArkRcon.Connect(server);
            }, true));
        }
    }
}
