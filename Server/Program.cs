using System;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            ChatStream.Server server = new ChatStream.Server(5500);
            server.Start();
        }
    }
}
