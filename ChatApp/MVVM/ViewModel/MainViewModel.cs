using ChatApp.MVVM.Model;
using ChatApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using static System.Net.WebRequestMethods;

namespace ChatApp.MVVM.ViewModel
{
    public  class MainViewModel : ObservableObject
    {
        public ObservableCollection<Message> Messages { get; set; }
        public ObservableCollection<Contact> Contacts { get; set; }

        private Contact _SelectedContact;



        public ICommand SendCommand { get; set; }

        private string _message;


        private string _currentUser;

        public string CurrentUser
        {
            get { return _currentUser; }
            set { _currentUser = value; OnPropertyChanged(); }
        }


        public string Message
        {
            get { return _message; }
            set { _message = value; OnPropertyChanged(); }
        }


        public Contact SelectedContact
        {
            get { return _SelectedContact; }
            set { _SelectedContact = value; OnPropertyChanged(); }
        }

        private bool isConnected;

        public bool IsConnected
        {
            get { return isConnected; }
            set { isConnected = value; OnPropertyChanged(); }
        }


        private Server _server;
        public ICommand SendImageCommand { get; set; }
        public MainViewModel()
        {
            IsConnected = false;
            Messages = new ObservableCollection<Message>();
            Contacts = new ObservableCollection<Contact>();
            SendCommand = new RelayCommand(SendMessage);
            SendImageCommand = new RelayCommand(SendImage);
            _server = new Server();

            _server.connectedEvent += UserConnected;
            _server.messageReceivedEvent += MessageReceived;
            _server.UserDisconnectReceivedEvent += UserDisconnected;
            _server.imageReceivedEvent += ImageReceived;
            CurrentUser = WindowsIdentity.GetCurrent().Name.Split('\\')[1];
            _server.Connect(CurrentUser);
            IsConnected = true;
        }

        public void SendImage()
        {
            _server.SendImageToServer();
        }

        public void SendMessage()
        {
            bool isFirst = true;
            if (Messages.Count > 0)
            {
                isFirst = false;
            }
            _server.SendMessageToServer(Message);
            Message = "";
        }

        private void UserConnected()
        {
            var user = new Contact
            {
                Username = _server._reader.ReadMessage(),
                UUID = _server._reader.ReadMessage(),
                Messages = new ObservableCollection<Message>(),
                ImageSource = "Images/user.png"
            };
            if(!Contacts.Any(x => x.UUID == user.UUID))
                Application.Current.Dispatcher.Invoke(() => Contacts.Add(user));
        }

        private void UserDisconnected()
        {
            var uid = _server._reader.ReadMessage();
            var user = Contacts.Where(x => x.UUID == uid).FirstOrDefault();
            Application.Current.Dispatcher.Invoke(() => Contacts.Remove(user));
        }
        private void MessageReceived()
        {
            var msg = _server._reader.ReadMessage();
            bool isFirst = true;
            if (Messages.Count > 0)
            {
                isFirst = false;
            }
            Application.Current.Dispatcher.Invoke(() => Messages.Add(new Model.Message
            {
                Content = msg,
                FirstMessage = isFirst
            }));
        }

        private void ImageReceived()
        {
            Debug.WriteLine("Image received!");
            var img = _server._reader.ReadImage();
            Application.Current.Dispatcher.Invoke(() => Messages.Add(new Model.Message
            {
                Content = img,
                IsImage = true
            }));
        }
    }
}
