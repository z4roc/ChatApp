using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.Net.IO
{
    public class PacketReader : BinaryReader
    {
        private NetworkStream _stream;
        public PacketReader(NetworkStream ns) : base(ns)
        {
            _stream = ns;
        }

        public string ReadMessage()
        {
            byte[] msgBuffer;
            var length = ReadInt32();
            msgBuffer = new byte[length];

            _stream.Read(msgBuffer, 0, length);

            var msg = Encoding.ASCII.GetString(msgBuffer);
            return msg;
        }

        public byte[] ReadImage()
        {
            var buffersize = 10485760; //10MB Daten
            byte[] msgBuffer = new byte[buffersize];
            var imagebytes = _stream.Read(msgBuffer, 0 , buffersize);
            return msgBuffer;
            
        }
    }
}
