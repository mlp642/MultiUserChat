using ChatUI.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ChatUI.MVVM.ViewModel
{
    class MainViewModel
    {
        public ObservableCollection<UserModel> users { get; set; }
        public ObservableCollection<string> messages { get; set; }
        public RelayCommand ConnectToServerCommand { get; set; }
        public RelayCommand sendMessageCommand { get; set; }
        public string userName { get; set; }
        public string message { get; set; }
        private Server _server;

        public MainViewModel()
        {
            users = new ObservableCollection<UserModel>();
            messages = new ObservableCollection<string>();
            _server = new Server();
            _server.connectedEvent += UserConnected;
            _server.msgReceivedEvent += MessageReceived;
            _server.userDisconnectEvent += RemoveUser;
            ConnectToServerCommand = new RelayCommand(o => _server.ConnectToServer(userName), o => !string.IsNullOrEmpty(userName));
            sendMessageCommand = new RelayCommand(o => _server.SendMessageToServer(message), o => !string.IsNullOrEmpty(message));
        }

        private void RemoveUser()
        {
            var uid = _server.packetReader.ReadMessage();
            var user = users.Where(x => x.UID == uid).FirstOrDefault();
            Application.Current.Dispatcher.Invoke(() => users.Remove(user));
        }

        private void MessageReceived()
        {
            var msg = _server.packetReader.ReadMessage();
            Application.Current.Dispatcher.Invoke(() => messages.Add(msg));
        }

        private void UserConnected()
        {
            var user = new UserModel
            {
                UserName = _server.packetReader.ReadMessage(),
                UID = _server.packetReader.ReadMessage(),
            };
            if (!users.Any(x => x.UID == user.UID))
            {
                Application.Current.Dispatcher.Invoke(() => users.Add(user));
            }
        }
    }
}
