using Ark.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ark
{
    public class Client: IDisposable
    {
        private TcpClient TcpClient {get; set;}
        private NetworkStream TcpClientStream {get; set;}
        private Queue<Packet> OutgoingPackets {get; set;}
        //private Keepalive Keepalive {get; set;}
        private bool CanSendPacket
        {
            get
            {
                return OutgoingPacketCooldown.ElapsedMilliseconds >= 800 || IncomingPacketReceieved;
            }
        }
        private bool IncomingPacketReceieved {get; set;}
        private Stopwatch OutgoingPacketCooldown {get; set;}

        public event EventHandler<ServerConnectionEventArgs> ServerConnectionFailed;
        public event EventHandler<ServerConnectionEventArgs> ServerConnectionDropped;
        public event EventHandler<ServerConnectionEventArgs> ServerConnectionSucceeded;
        public event EventHandler<ServerConnectionEventArgs> ServerConnectionStarting;
        public event EventHandler<ServerConnectionEventArgs> ServerConnectionDisconnected;
        public event EventHandler<PacketEventArgs> ReceivedPacket;

        public Client()
        {
            OutgoingPackets = new Queue<Packet>();
            //Keepalive = new Keepalive(this);
            OutgoingPacketCooldown = new Stopwatch();
            OutgoingPacketCooldown.Restart();
            IncomingPacketReceieved = true;
        }

        public bool IsConnected
        {
            get
            {
                return TcpClient != null && TcpClient.Connected;
            }
        }

        public void SendPacket(Packet packet)
        {
            OutgoingPackets.Enqueue(packet);
        }

        public async Task<bool> Connect(string hostname, int port)
        {
            if(ServerConnectionStarting != null)
                ServerConnectionStarting(this, new ServerConnectionEventArgs{Message = "Connecting to " + hostname + ":" + port.ToString() + "...", Status = ServerConnectionStatus.Connecting, Timestamp = DateTime.Now});

            try
            {
                TcpClient = new TcpClient();
                TcpClient.NoDelay = true;
                await TcpClient.ConnectAsync(hostname, port);
            }
            catch
            {
                if(ServerConnectionFailed != null)
                    ServerConnectionFailed(this, new ServerConnectionEventArgs{Message = "Failed to connect!  Make sure the server is running and that your hostname and port are correct.", Status = ServerConnectionStatus.Disconnected, Timestamp = DateTime.Now});
            }
            if(IsConnected)
            {
                TcpClientStream = TcpClient.GetStream();

                if(ServerConnectionSucceeded != null)
                    ServerConnectionSucceeded(this, new ServerConnectionEventArgs{Message = "Successfully connected.", Status = ServerConnectionStatus.Connected, Timestamp = DateTime.Now});

                //Keepalive.Reset();
                return true;
            }
            else return false;
        }

        public void Disconnect()
        {
            if(ServerConnectionDisconnected != null)
                ServerConnectionDisconnected(this, new ServerConnectionEventArgs{Message = "Disconnected from server.", Status = ServerConnectionStatus.Disconnected, Timestamp = DateTime.Now});
        }

        public async Task Update()
        {
            if(IsConnected)
            {
                await ProcessPacketStream();
                //Keepalive.Update();
            }
        }


        private async Task ProcessPacketStream()
        {
            try
            {
                while(IsConnected && (OutgoingPackets.Count != 0 || TcpClient.Available != 0))
                {
                    while(IsConnected && TcpClient.Available != 0)
                    {
                        if(ReceivedPacket != null)
                            ReceivedPacket(this, new PacketEventArgs(){Packet = await Packet.ReadFromStreamAsync(TcpClientStream)});

                        IncomingPacketReceieved = true;
                    }


                    if(IsConnected && CanSendPacket && OutgoingPackets.Count != 0)
                    {
                        await OutgoingPackets.Dequeue().WriteToStreamAsync(TcpClientStream);
                        OutgoingPacketCooldown.Restart();
                        IncomingPacketReceieved = false;
                    }

                    //// We've successfully sent or recieved data so the keepalive can be pushed back.
                    //Keepalive.Reset();
                }
            }
            catch
            {
                // Lost connection with the server
                // No handling is necessary here as the TCPClient will set Connected to false.
                if(ServerConnectionDropped != null)
                    ServerConnectionDropped(this, new ServerConnectionEventArgs(){Message = "Connection to the server has been lost.", Status = ServerConnectionStatus.Disconnected, Timestamp = DateTime.Now});
            }
        }

        public void Dispose()
        {
            if(TcpClientStream != null)
            {
                TcpClientStream.Dispose();
                TcpClientStream = null;
            }

            if(TcpClient != null)
            {
                TcpClient.Close();
                TcpClient = null;
            }

            OutgoingPackets.Clear();
        }
    }
}
