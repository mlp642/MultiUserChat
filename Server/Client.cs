using System;
using System.Net.Sockets;
using System.Threading.Tasks;

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
        Task.Run(() => Process());
    }

    void Process()
    {
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
                        ServerMain.BroadcastMessage($"[{DateTime.Now}]: {Username}]: {msg}");
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"[{UID.ToString()}]: Desconectado!");
                ServerMain.BroadcastDisconnect(UID.ToString());
                ClientSocket.Close();
                break;
            }
        }
    }
}
