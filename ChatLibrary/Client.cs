using System;
using System.Net.Sockets;
using System.Text;

namespace ChatStream
{
    public class Client
    {
        private TcpClient client;
        private NetworkStream stream;

        public event EventHandler<string> MessageReceived;
        public event EventHandler Disconnected;

        public Client()
        {
            client = new TcpClient();
        }

        public void Connect(string ipAddress, int port)
        {
            client.Connect(ipAddress, port);
            stream = client.GetStream();
        }

        public void StartReceivingMessages()
        {
            byte[] buffer = new byte[1024];
            int bytesRead;

            try
            {
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    MessageReceived?.Invoke(this, message);
                }
            }
            catch (Exception)
            {
                Disconnected?.Invoke(this, EventArgs.Empty);
            }
            finally
            {
                stream.Close();
                client.Close();
            }
        }

        public void SendMessage(string message)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            stream.Write(buffer, 0, buffer.Length);
        }

        public void SendUsername(string username)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(username);
            stream.Write(buffer, 0, buffer.Length);
        }
    }
}
