using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ChatStream
{
    public class Server
    {
        private TcpClient _client;

        public Server()
        {
            _client = new TcpClient();
        }

        public void ConnectToServer(string userName)
        {

            if (!_client.Connected)
            {

                _client.Connect("127.0.0.1", 7890);
                var connectPacket = new PacketBuilder();
                connectPacket.WriteOpCode(0);
                connectPacket.WriteString(userName);
                _client.Client.Send(connectPacket.GetPacketBytes());
            }
        }

    }
}
