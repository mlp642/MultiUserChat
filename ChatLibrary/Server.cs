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
        private Dictionary<string, TcpClient> clients = new Dictionary<string, TcpClient>();

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
                    TcpClient tcpClient = listener.AcceptTcpClient();
                    Console.WriteLine("Cliente conectado. Solicitando nombre de usuario...");

                    NetworkStream stream = tcpClient.GetStream();
                    byte[] buffer = new byte[1024];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string username = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                    while (clients.ContainsKey(username))
                    {
                        byte[] usernameExistMessage = Encoding.ASCII.GetBytes("El nombre de usuario ya está en uso. Introduzca otro nombre de usuario:");
                        stream.Write(usernameExistMessage, 0, usernameExistMessage.Length);
                        bytesRead = stream.Read(buffer, 0, buffer.Length);
                        username = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    }

                    byte[] usernameConfirmMessage = Encoding.ASCII.GetBytes("Usuario registrado con éxito. Puedes empezar a chatear.");
                    stream.Write(usernameConfirmMessage, 0, usernameConfirmMessage.Length);

                    clients.Add(username, tcpClient);

                    Thread clientThread = new Thread(HandleClient);
                    clientThread.Start(username);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en el servidor: {ex.Message}");
            }
        }

        private void HandleClient(object obj)
        {
            string username = (string)obj;
            TcpClient client = clients[username];
            NetworkStream stream = client.GetStream();

            byte[] buffer = new byte[1024];
            int bytesRead;

            try
            {
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    string message = $"{username}: {Encoding.ASCII.GetString(buffer, 0, bytesRead)}";
                    Console.WriteLine($"Mensaje recibido: {message}");

                    // Reenviar el mensaje a todos los clientes conectados
                    BroadcastMessage(message);
                }
            }
            catch (Exception)
            {
                // Si ocurre un error al leer del cliente, asumimos que se desconectó
                Console.WriteLine($"Cliente {username} desconectado.");
                clients.Remove(username);
            }
        }

        private void BroadcastMessage(string message)
        {
            foreach (TcpClient client in clients.Values)
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = Encoding.ASCII.GetBytes(message);
                stream.Write(buffer, 0, buffer.Length);
                stream.Flush();
            }
        }
    }
}
