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
        private TcpListener listener;
        private List<TcpClient> clients = new List<TcpClient>();

        public Server(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
        }

        public void Start()
        {
            try
            {
                listener.Start();
                Console.WriteLine("Servidor de chat iniciado. Esperando conexiones...");

                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    clients.Add(client);

                    Thread clientThread = new Thread(HandleClient);
                    clientThread.Start(client);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en el servidor: {ex.Message}");
            }
        }

        private void HandleClient(object obj)
        {
            TcpClient client = (TcpClient)obj;
            NetworkStream stream = client.GetStream();

            byte[] buffer = new byte[1024];
            int bytesRead;

            try
            {
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"Mensaje recibido: {message}");

                    // Reenviar el mensaje a todos los clientes conectados
                    BroadcastMessage(message);
                }
            }
            catch (Exception)
            {
                // Si ocurre un error al leer del cliente, asumimos que se desconectó
                Console.WriteLine("Cliente desconectado.");
                clients.Remove(client);
            }
        }

        private void BroadcastMessage(string message)
        {
            foreach (TcpClient client in clients)
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = Encoding.ASCII.GetBytes(message);
                stream.Write(buffer, 0, buffer.Length);
                stream.Flush();
            }
        }
    }
}
