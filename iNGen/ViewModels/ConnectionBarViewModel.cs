using PTK.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iNGen.ViewModels
{
    public class ConnectionBarViewModel: Notifiable
    {
        private string mConnectionStatus;
        public string ConnectionStatus
        {
            get {return mConnectionStatus;}
            set {SetField(ref mConnectionStatus, value);}
        }

        private string mServerDisplayName;
        public string ServerDisplayName
        {
            get {return mServerDisplayName;}
            set {SetField(ref mServerDisplayName, value);}
        }

        private string mCurrentPlayerCount;
        public string CurrentPlayerCount
        {
            get {return mCurrentPlayerCount;}
            set {SetField(ref mCurrentPlayerCount, value);}
        }

        private string mMaxPlayerCount;
        public string MaxPlayerCount
        {
            get {return mMaxPlayerCount;}
            set {SetField(ref mMaxPlayerCount, value);}
        }

        public ConnectionBarViewModel()
        {
            ConnectionStatus = Ark.ServerConnectionStatus.Disconnected.ToString();
            ServerDisplayName = "NO SERVER CONNECTION";
            CurrentPlayerCount = "0";
            MaxPlayerCount = "0";

            App.ArkRcon.ServerConnectionStarting +=        (s, args) => ConnectionStatus = args.Status.ToString();
            App.ArkRcon.ServerConnectionSucceeded +=       (s, args) => ConnectionStatus = args.Status.ToString();
            App.ArkRcon.ServerConnectionFailed +=          (s, args) => ConnectionStatus = args.Status.ToString();
            App.ArkRcon.ServerConnectionDisconnected +=    (s, args) => ConnectionStatus = args.Status.ToString();
            App.ArkRcon.ServerConnectionDropped +=         (s, args) => ConnectionStatus = args.Status.ToString();
            App.ArkRcon.HostnameUpdated +=                 (s, args) => ServerDisplayName = args.NewHostname;
            App.ArkRcon.CurrentPlayerCountUpdated +=       (s, args) => CurrentPlayerCount = args.PlayerCount.ToString();
        }
    }
}
