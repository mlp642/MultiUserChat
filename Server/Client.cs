using System;
using System.Net.Sockets;

public class Client
{
    public string Username { get; set; }
    public Guid UID { get; set; }
    public TcpClient ClientSocket { get; set; }
    PacketReader packetReader;

    public Client(TcpClient client)
    {
        ClientSocket = client;
        UID = Guid.NewGuid();
        packetReader = new PacketReader(ClientSocket.GetStream());
        var opcode = packetReader.ReadByte();
        Username = packetReader.ReadMessage();
        Console.WriteLine($"[{DateTime.Now}]: Un cliente se ha conectado con el usuario: {Username}");
    }



    void Process() {
        while (true)
        {

            try
            {
                var opcode = packetReader.ReadByte();
                switch (opcode) 
                {

                    case 5:
                        var msg = packetReader.ReadMessage();
                        Console.WriteLine($"[{DateTime.Now}]: Mensaje recibido! {msg}");
                        Program.BroadcastMessage( msg );
                        break;
                    default:
                        break;
                } 




            }catch (Exception e)
            {

                Console.WriteLine($"[{UID.ToString()}]: Desconectado!");
                ClientSocket.Close();
                throw;

            }
        }
    }
}
