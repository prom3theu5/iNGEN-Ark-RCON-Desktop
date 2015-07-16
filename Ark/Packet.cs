using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Ark
{
    public enum PacketType
    {
        ResponseValue = 0,
        ExecCommand = 2,
        AuthResponse = 2,
        Auth = 3,
        Server = 4
    }
    
    public class Packet
    {
        public Opcode Opcode {get; set;}
        public PacketType Type {get; set;}
        public byte[] Data {get; set;}
        public DateTime Timestamp {get; set;}

        public Packet()
        {
            Timestamp = DateTime.Now;
        }

        public Packet(Opcode opcode, PacketType type, byte[] data): this()
        {
            Opcode = opcode;
            Type = type;
            Data = data;
        }
        public Packet(Opcode opcode, PacketType type, string data): this(opcode, type, Encoding.Default.GetBytes(data + "\0")){}
        public Packet(Opcode opcode, PacketType type, int data): this(opcode, type, data.ToString()){}
        public Packet(Opcode opcode, PacketType type): this(opcode, type, ""){}

        public string DataAsString()
        {
            return Encoding.Default.GetString(Data, 0, Data.Length - 1);
        }

        public int DataSize
        {
            get
            {
                return Data.Length;
            }
            set
            {
                Data = new byte[value];
            }
        }

        public int PacketSize
        {
            get
            {
                return
                    sizeof(Opcode) +
                    sizeof(PacketType) +
                    DataSize +
                    sizeof(byte); // Packet terminator (0x00)
            }
            set
            {
                DataSize = value - sizeof(Opcode) - sizeof(PacketType) - sizeof(byte);
            }
        }

        public int TotalPacketSize
        {
            get
            {
                return
                    sizeof(int) + // Integer to hold PacketSize
                    PacketSize;
            }
        }

        public async Task WriteToStreamAsync(NetworkStream stream)
        {
            byte[] packet = new byte[TotalPacketSize];
            
            BitConverter.GetBytes(PacketSize).CopyTo(packet, 0);
            BitConverter.GetBytes((int)Opcode).CopyTo(packet, 4);
            BitConverter.GetBytes((int)Type).CopyTo(packet, 8);
            Data.CopyTo(packet, 12);
            packet[packet.Length - 1] = ((byte)0x00);

            Console.WriteLine("Sent {0}", DataAsString());
            await stream.WriteAsync(packet, 0, TotalPacketSize);
        }

        public static async Task<Packet> ReadFromStreamAsync(NetworkStream stream)
        {
            Packet packet = new Packet();

            byte[] sizeBuffer = new byte[4];
            stream.Read(sizeBuffer, 0, 4);
            packet.PacketSize = BitConverter.ToInt32(sizeBuffer, 0);

            byte[] packetBuffer = new byte[packet.PacketSize];
            if(packet.PacketSize != 0)
            {
                int readProgress = 0;
                while(readProgress < packet.PacketSize)
                {
                    int read = await stream.ReadAsync(packetBuffer, readProgress, packet.PacketSize - readProgress);
                    readProgress += read;

                    if(read == 0)
                    {
                        throw new Exception("NetworkStream failed to read data.  Connection may have been lost!");
                    }
                }
            }

            packet.Opcode = (Opcode)BitConverter.ToInt32(packetBuffer, 0);
            packet.Type = (PacketType)BitConverter.ToInt32(packetBuffer, 4);
            Array.Copy(packetBuffer, 8, packet.Data, 0, packet.DataSize);

            Console.WriteLine("Recieved {0}", packet.DataAsString());
            return packet;
        }

    }
}
