using Ark.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ark
{
    public class ServerResponseEventArgs: EventArgs
    {
        public string Message {get; set;}
        public DateTime Timestamp {get; set;}
    }

    public class HostnameEventArgs: EventArgs
    {
        public string NewHostname {get; set;}
        public DateTime Timestamp {get; set;}
    }

    public class PlayerCountEventArgs: EventArgs
    {
        public int PlayerCount {get; set;}
    }

    public class ConsoleLogEventArgs: EventArgs
    {
        public string Message {get; set;}
        public DateTime Timestamp {get; set;}
    }

    public class ChatLogEventArgs : EventArgs
    {
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsAdmin { get; set; }
        public string Sender { get; set; }
    }

    public class PlayersEventArgs: EventArgs
    {
        public List<Player> Players {get; set;}
    }

    public class ServerConnectionEventArgs: EventArgs
    {
        public string Message {get; set;}
        public ServerConnectionStatus Status {get; set;}
        public ConnectionInfo ConnectionInfo {get; set;}
        public DateTime Timestamp {get; set;}
    }

    public class PacketEventArgs: EventArgs
    {
        public Packet Packet {get; set;}
    }

    public class ServerAuthEventArgs: EventArgs
    {
        public string Message {get; set;}
        public DateTime Timestamp {get; set;}
    }
}
