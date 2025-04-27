using Queen.Network.Common;
using TouchSocket.Sockets;

namespace Queen.Network.Common.Channels;

/// <summary>
/// TCPServ 通信渠道
/// </summary>
/// <param name="socket">Socket</param>
public class TCPServerC(TcpSessionClient socket) : NetChannel<TcpSessionClient>(socket)
{
    public override string id { get => socket.Id; }
        
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