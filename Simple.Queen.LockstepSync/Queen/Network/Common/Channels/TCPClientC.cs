using Queen.Network.Common;
using Queen.Protocols.Common;
using TouchSocket.Sockets;

namespace Queen.Network.Common.Channels;

/// <summary>
/// TCPClient 通信渠道
/// </summary>
/// <param name="socket">Socket</param>
public class TCPClientC(TcpClient socket) : NetChannel<TcpClient>(socket)
{
    public override string id { get => ""; }

    public override bool alive { get => socket.Online; }

    public override void Send(byte[] data)
    {
        if (false == alive) return;
        socket.SendAsync(data);
    }
        
    public override void Disconnect()
    {
        if (false == alive) return;
        socket.Close();
    }
}