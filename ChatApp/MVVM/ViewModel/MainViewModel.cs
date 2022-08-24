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
            
        }

        public void Connect() => _server.Connect(CurrentUser);
        public void SendMessage()
        {
            Messages.Add(new Message
            {
                Content = Message,
                FirstMessage = false
            });
        }
    }
}
