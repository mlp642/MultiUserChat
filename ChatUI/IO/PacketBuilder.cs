using System.IO;
using System;
using System.Text;

public class PacketBuilder
{
    MemoryStream _ms;

    public PacketBuilder()
    {
        _ms = new MemoryStream();
    }

    public void WriteOpCode(byte opcode)
    {
        _ms.WriteByte(opcode);
    }

    public void WriteMessage(string msg)
    {
        var msgLength = msg.Length;
        _ms.Write(BitConverter.GetBytes(msgLength), 0, 4);

        // Convertir el mensaje a bytes
        byte[] msgBytes = Encoding.UTF8.GetBytes(msg);

        // Escribir los bytes en el MemoryStream
        _ms.Write(msgBytes, 0, msgBytes.Length);
    }

    public byte[] GetPacketBytes()
    {
        return _ms.ToArray();
    }
}
