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

        public ObservableCollection<UserModel> users {  get; set; }
        public RelayCommand ConnectToServerCommand { get; set; }

        public RelayCommand sendMessageCommand { get; set; }

        public string userName { get; set; }
        public string message { get; set; }

        private Server _server;

        public MainViewModel()
        {
            users = new ObservableCollection<UserModel>();
            _server = new Server();
            _server.connectedEvent += UserConnected;
            ConnectToServerCommand = new RelayCommand(o => _server.ConnectToServer(userName), o => !string.IsNullOrEmpty(userName));
            sendMessageCommand = new RelayCommand(o => _server.SendMessageToServer(message), o => !string.IsNullOrEmpty(message));
        }

        private void UserConnected()
        {
            var user = new UserModel
            {
                UserName = _server.packetReader.ReadMessage(),
                UID = _server.packetReader.ReadMessage(),

            };

            if(!users.Any(x  => x.UID == user.UID) ) 
            {
                Application.Current.Dispatcher.Invoke(() => users.Add(user)); 
            }
        }
    }
}
