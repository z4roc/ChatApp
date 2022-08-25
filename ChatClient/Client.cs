using ChatClient.Net.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient
{
    public class Client
    {
        public string Username { get; set; }
        public Guid UID { get; set; }
        public TcpClient ClientSocket { get; set; }

        PacketReader _reader;
        public Client(TcpClient client)
        {
            ClientSocket = client;
            _reader = new PacketReader(ClientSocket.GetStream());
            UID = Guid.NewGuid();

            var opcode = _reader.ReadByte();
            Username = _reader.ReadMessage();
            
            Console.WriteLine($"{DateTime.Now} {Username} Connected");
            Task.Run(() =>
            {
                Process();
            });
        }

        void Process()
        {
            while (true)
            {
                try
                {
                    var opcode = _reader.ReadByte();
                    switch (opcode)
                    {
                        case 5:
                            var msg = _reader.ReadMessage();
                            Console.WriteLine($"{DateTime.Now} Message Received {msg}");
                            Program.BroadcastMessage($"[{DateTime.Now}]: [{Username}]: " +msg);
                            break;
                        case 6:
                            var image =_reader.ReadImage();
                            Console.WriteLine("Image Received   " /*+ imageBytes.ToString()*/);
                            Program.BroadCastImage(image);
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine($"{UID} Disconnected");
                    Program.BroadcastDisconnect(UID.ToString());
                    ClientSocket.Close();
                    break;
                }
            }
        }
    }
}
