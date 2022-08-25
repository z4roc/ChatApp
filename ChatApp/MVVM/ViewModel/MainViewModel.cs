using ChatApp.MVVM.Model;
using ChatApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
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

        private Server _server;
        public ICommand ConnectToServerCommand { get; set; }

        public MainViewModel()
        {
            Messages = new ObservableCollection<Message>();
            Contacts = new ObservableCollection<Contact>();
            SendCommand = new RelayCommand(SendMessage);
            ConnectToServerCommand = new RelayCommand(Connect);
            _server = new Server();

            _server.connectedEvent += UserConnected;
            _server.messageReceivedEvent += MessageReceived;
            _server.UserDisconnectReceivedEvent += UserDisconnected;
        }

        public void Connect() => _server.Connect(CurrentUser);
        public void SendMessage()
        {
            bool isFirst = true;
            if (Messages.Count > 0)
            {
                isFirst = false;
            }
            _server.SendMessageToServer(Message);
        }

        private void UserConnected()
        {
            var user = new Contact
            {
                Username = _server._reader.ReadMessage(),
                UUID = _server._reader.ReadMessage(),
                Messages = new ObservableCollection<Message>()
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
    }
}
