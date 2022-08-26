using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ChatApp.MVVM.Model
{
    public class Message
    {
        public string Username { get; set; }
        public string UsernameColor { get; set; }
        public string ImageSource { get; set; }
        public string Content { get; set; }
        public DateTime Time { get; set; }
        public bool IsNativeOrigin { get; set; }
        public bool? FirstMessage { get; set; }
        public bool? IsImage { get; set; }
        public BitmapImage? Image { get; set; }

    }
}
