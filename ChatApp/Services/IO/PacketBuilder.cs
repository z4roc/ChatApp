using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ChatApp.Services.IO
{
    public class PacketBuilder
    {
        //19:24
        //https://www.youtube.com/watch?v=I-Xmp-mulz4
        MemoryStream ms;

        public PacketBuilder()
        {
            ms = new MemoryStream();
        }

        public void WriteOpCode(byte opcode)
        {
            ms.WriteByte(opcode);
        }

        public void WriteMessage(string msg)
        {
            var msgLength = msg.Length;
            ms.Write(BitConverter.GetBytes(msgLength));
            ms.Write(Encoding.ASCII.GetBytes(msg));
        }

        public void WriteImage(string imagepath)
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri(imagepath);
            bi.EndInit();
            byte[] imgbytes;
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bi));
            using (var tempstream = new MemoryStream())
            {
                encoder.Save(tempstream);
                imgbytes = tempstream.ToArray();
            }
              

            ms.Write(BitConverter.GetBytes(imgbytes.Length));
            ms.Write(imgbytes);
                
        }

        public byte[] GetPacketBytes()
        {
            return ms.ToArray();
        }
    }
}
