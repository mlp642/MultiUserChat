using System;
using System.Threading;

namespace Client
{
    class ClientMain
    {
        static void Main(string[] args)
        {
            ChatStream.Client client = new ChatStream.Client("127.0.0.1", 5500);
            Console.WriteLine("Bienvenido al chat! Ingrese su nombre de usuario:");

            // Iniciar un hilo para recibir mensajes del servidor
            var receivingThread = new Thread(client.StartReceivingMessages);
            receivingThread.Start();

            while (true)
            {
                string message = Console.ReadLine();
                client.SendMessage(message);
            }
        }
    }
}
