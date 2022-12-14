using ChatApp.Services.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Services
{
    public class Server
    {
        TcpClient _client;
        public PacketReader _reader;

        public event Action connectedEvent;
        public event Action messageReceivedEvent;
        public event Action UserDisconnectReceivedEvent;
        public event Action imageReceivedEvent;
        public Server()
        {
            _client = new TcpClient();
        }

        public void Connect(string username)
        {
            if (!_client.Connected) _client.Connect("10.11.6.100", 7891);
            _reader = new PacketReader(_client.GetStream());

            if (!string.IsNullOrEmpty(username))
            {
                
                var connectPacket = new PacketBuilder();

                connectPacket.WriteOpCode(0);
                connectPacket.WriteMessage(username);
                _client.Client.Send(connectPacket.GetPacketBytes());
            }
            ReadPackets();
        }

        private void ReadPackets()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    var opcode = _reader.ReadByte();
                    switch (opcode)
                    {
                        case 1:
                            connectedEvent?.Invoke();
                            break;
                        case 5:
                            messageReceivedEvent?.Invoke();
                            break;
                        case 6:
                            imageReceivedEvent?.Invoke();
                            break;
                        default:
                            Debug.WriteLine("wtf");
                            break;
                    }
                }
            });
        }

        public void SendMessageToServer(string message)
        {
            var messagePacket = new PacketBuilder();
            messagePacket.WriteOpCode(5);
            messagePacket.WriteMessage(message);
            _client.Client.Send(messagePacket.GetPacketBytes());
        }

        internal void SendImageToServer(string filename)
        {
            var imagePacket = new PacketBuilder();
            imagePacket.WriteOpCode(6);
            imagePacket.WriteImage(filename);
            _client.Client.Send(imagePacket.GetPacketBytes());
        }
    }
}
