using PTK.ModelManager;
using PTK.WPF;
using ProtoBuf;
using Ark.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iNGen.Models
{
    [Model("Servers")]
    [ProtoContract]
    public class ServerModelSet: Notifiable
    {
        [ProtoMember(1)]
        public ObservableCollection<Server> Servers {get; set;}
        public Server ConnectedServer {get; set;}

        public ServerModelSet()
        {
            Servers = new ObservableCollection<Server>();
        }

        public void ToggleDefault(Server server)
        {
            if(server.IsDefault)
            {
                server.IsDefault = false;
            }
            else
            {
                foreach(var s in Servers)
                {
                    s.IsDefault = false;
                }
                server.IsDefault = true;
            }
            App.ModelManager.Save<ServerModelSet>();
        }

        public Server NewServer()
        {
            Server server = new Server();
            Servers.Add(server);
            
            App.ModelManager.Save<ServerModelSet>();
            return server;
        }

        public void DeleteServer(Server server)
        {
            Servers.Remove(server);
            App.ModelManager.Save<ServerModelSet>();
        }
    }
    
    [ProtoContract]
    public class Server: Notifiable, ConnectionInfo
    {
        private string mDisplayName;
        private string mHostname;
        private int mPort;
        private string mPassword;
        private bool mIsDefault;
        private bool mIsConnected;

        public Server()
        {
            DisplayName = "New Server";
            Hostname = "";
            Port = 0;
            Password = "";
            IsDefault = false;
        }

        [ProtoMember(1)]
        public string DisplayName
        {
            get {return mDisplayName;}
            set {SetField(ref mDisplayName, value);}
        }
        
        [ProtoMember(2)]
        public string Hostname
        {
            get {return mHostname;}
            set {SetField(ref mHostname, value);}
        }
        
        [ProtoMember(3)]
        public int Port
        {
            get {return mPort;}
            set {SetField(ref mPort, value);}
        }
        
        [ProtoMember(4)]
        public string Password
        {
            get {return mPassword;}
            set {SetField(ref mPassword, value);}
        }

        [ProtoMember(5)]
        public bool IsDefault
        {
            get {return mIsDefault;}
            set {SetField(ref mIsDefault, value);}
        }

        public bool IsConnected
        {
            get {return mIsConnected;}
            set {SetField(ref mIsConnected, value);}
        }
    }
}
