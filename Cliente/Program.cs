using System;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            ChatStream.Client client = new ChatStream.Client("127.0.0.1", 5500);
            Console.WriteLine("Conectado al servidor. Ingrese mensajes:");

            // Iniciar un hilo para recibir mensajes del servidor
            var receivingThread = new System.Threading.Thread(client.StartReceivingMessages);
            receivingThread.Start();

            while (true)
            {
                string message = Console.ReadLine();
                client.SendMessage(message);
            }
        }
    }
}
