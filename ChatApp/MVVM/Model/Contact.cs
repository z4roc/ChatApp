using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.MVVM.Model
{
    public class Contact
    {
        public string Username { get; set; }
        public string UUID { get; set; }
        public string ImageSource { get; set; }
        public ObservableCollection<Message> Messages { get; set; }
        public string LastMessage => "Hello";
    }
}
