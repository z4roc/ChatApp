using ChatClient.Net.IO;
using System.Net;
using System.Net.Sockets;

namespace ChatClient
{
    class Program
    {
        static TcpListener _listener;
        static List<Client> _users;
        static void Main(string[] args)
        {
            _users = new List<Client>();
            _listener = new TcpListener(IPAddress.Parse("10.11.6.100"), 7891);
            _listener.Start();
            while (true)
            {
                var client = new Client(_listener.AcceptTcpClient());
                _users.Add(client);

                BroadCastConnection();
            }
        }

        static void BroadCastConnection()
        {
            foreach (var user in _users)
            {
                foreach (var usr in _users)
                {
                    var broadcastPacket = new PacketBuilder();
                    broadcastPacket.WriteOpCode(1);

                    broadcastPacket.WriteMessage(usr.Username);
                    broadcastPacket.WriteMessage(usr.UID.ToString());
                    user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
                }
            }
        }

        public static void BroadcastMessage(string message)
        {
            foreach(var user in _users)
            {
                var msgPacket = new PacketBuilder();
                msgPacket.WriteOpCode(5);
                msgPacket.WriteMessage(message);
                user.ClientSocket.Client.Send(msgPacket.GetPacketBytes());
            }
        }

        public static void BroadCastImage(byte[] imageBytes)
        {
            foreach (var item in _users)
            {
                var imagePacket = new PacketBuilder();
                imagePacket.WriteOpCode(6);
                imagePacket.WriteImageBytes(imageBytes);
                item.ClientSocket.Client.Send(imagePacket.GetPacketBytes());
            }
        }

        public static void BroadcastDisconnect(string UID)
        {
            var disconnecteduser = _users.Where(x => x.UID.ToString() == UID).FirstOrDefault();
            _users.Remove(disconnecteduser);
            foreach (var user in _users)
            {
                var BroadcastPacket = new PacketBuilder();
                BroadcastPacket.WriteOpCode(10);
                BroadcastPacket.WriteMessage(UID);
                user.ClientSocket.Client.Send(BroadcastPacket.GetPacketBytes());
            }

            BroadcastMessage(disconnecteduser.Username + " disconnected");
        }
    }
}
