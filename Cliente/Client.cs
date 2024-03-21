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
        Console.WriteLine($"[{DateTime.Now}]: Client has connected with Username: {Username}");
    }
}
