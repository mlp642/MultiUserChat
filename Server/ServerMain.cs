﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

class Program
{
    static List<Client> _users;
    static TcpListener listener;

    static void Main(string[] args)
    {
        _users = new List<Client>();
        listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 7890);
        listener.Start();

        while (true)
        {
            var client = new Client(listener.AcceptTcpClient());
            _users.Add(client);

            /* Broadcast the connection to everyone on the server */
            BroadcastConnection();
        }


    }
    static void BroadcastConnection()
    {
        foreach (var user in _users)
        {
            foreach (var usr in _users)
            {
                var broadcastPacket = new PacketBuilder();
                broadcastPacket.WriteOpCode(1);
                broadcastPacket.WriteMessage(usr.Username);
                broadcastPacket.WriteMessage(usr.UID.ToString());
                user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
            }
        }
    }

    public static void BroadcastMessage(string message)
    {

        foreach(var user in _users)
        {

            var msgPacket = new PacketBuilder();
            msgPacket.WriteOpCode(5);
            msgPacket.WriteMessage(message);
            user.ClientSocket.Client.Send(msgPacket.GetPacketBytes());
        }
    }

    public static void BroadcastDisconnect(string uid)
    {
        var disconnectedUser = _users.Where(x => x.UID.ToString() == uid).FirstOrDefault();
        _users.Remove(disconnectedUser);

        foreach (var user in _users)
        {

            var broadcastPacket = new PacketBuilder();
            broadcastPacket.WriteOpCode(10);
            broadcastPacket.WriteMessage(uid);
            user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());

          
        }
    }



}
