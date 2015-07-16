using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ark
{
    public class Keepalive
    {
        private Client Client {get; set;}
        private Stopwatch Timer {get; set;}
        private const int Interval = 2000;

        public Keepalive(Client client)
        {
            Client = client;
            Timer = new Stopwatch();
            Timer.Restart();
        }

        public void Update()
        {
            if(Timer.ElapsedMilliseconds > Interval)
            {
                
                Client.SendPacket(new Packet(Opcode.Keepalive, PacketType.ResponseValue));
                Reset();
            }
        }

        public void Reset()
        {
            Timer.Restart();
        }
    }
}
