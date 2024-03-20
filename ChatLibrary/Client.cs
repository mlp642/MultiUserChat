using System;
using System.Net.Sockets;
using System.Text;

namespace ChatStream
{
    public class Client
    {
        private TcpClient client;
        private NetworkStream stream;

        public Client(string ipAddress, int port)
        {
            client = new TcpClient(ipAddress, port);
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
                    string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"Mensaje recibido: {message}");
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Desconectado del servidor.");
            }
            finally
            {
                stream.Close();
                client.Close();
            }
        }

        public void SendMessage(string message)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(message);
            stream.Write(buffer, 0, buffer.Length);
        }
    }
}
