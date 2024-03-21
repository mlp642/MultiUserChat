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
                    Thread clientThread = new Thread(() => HandleClient(tcpClient));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en el servidor: {ex.Message}");
            }
        }

        private void HandleClient(TcpClient tcpClient)
        {
            NetworkStream stream = tcpClient.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string username = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            while (string.IsNullOrWhiteSpace(username) || clients.ContainsKey(username.ToLower()))
            {
                byte[] usernameExistMessage = Encoding.UTF8.GetBytes("El nombre de usuario ya está en uso. Introduzca otro nombre de usuario:");
                stream.Write(usernameExistMessage, 0, usernameExistMessage.Length);
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                username = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            }

            byte[] usernameConfirmMessage = Encoding.UTF8.GetBytes("Usuario registrado con éxito. Puedes empezar a chatear.");
            stream.Write(usernameConfirmMessage, 0, usernameConfirmMessage.Length);

            string newConnection = $"{username} se ha unido al chat.";
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(newConnection);
            BroadcastMessage(newConnection);

            clients.Add(username, tcpClient);

            try
            {
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    string message = $"{username}: {Encoding.UTF8.GetString(buffer, 0, bytesRead)}";
                    Console.ResetColor();
                    Console.WriteLine($"{message}");
                    BroadcastMessage(message);
                }
            }
            catch (Exception)
            {
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{username} se ha desconectado.");
                clients.Remove(username);
            }
        }

        private void BroadcastMessage(string message)
        {
            foreach (TcpClient client in clients.Values)
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = Encoding.UTF8.GetBytes(message);
                stream.Write(buffer, 0, buffer.Length);
                stream.Flush();
            }
        }
    }
}
