using LiteNetLib;
using Queen.Network.Common;
using System.Net;
using TouchSocket.Core;
using TouchSocket.Sockets;

namespace Queen.Network.Common.Channels;

/// <summary>
/// UDPNode 通信渠道
/// </summary>
/// <param name="socket">Socket</param>
public class UDPNodeC(NetPeer socket) : NetChannel<NetPeer>(socket)
{
    public override string id { get => $"{socket.Id}"; }

    public override bool alive { get => ConnectionState.Connected == socket.ConnectionState; }

    public override void Send(byte[] data)
    {
        socket.Send(data, DeliveryMethod.ReliableOrdered);
    }

    public override void Disconnect()
    {
        socket.Disconnect();
    }
}