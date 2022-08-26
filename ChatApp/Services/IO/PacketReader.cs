using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ChatApp.Services.IO
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

        public string ReadImage()
        {
            var buffersize = ReadInt32(); //10MB Daten
            byte[] imageData = new byte[buffersize];
            var imagebytes = _stream.Read(imageData, 0, buffersize);
            string imgpath = "";
            Bitmap bmp;
            using (var ms = new MemoryStream(imageData))
            {
                bmp = new Bitmap(ms);
                var imgname = Guid.NewGuid();
                imgpath = $"Images/{imgname}.png";
                bmp.Save(imgpath);
            }
            return imgpath;
        }
    }
}
