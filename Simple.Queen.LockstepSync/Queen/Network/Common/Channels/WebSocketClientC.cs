using TouchSocket.Sockets;
using WSC = TouchSocket.Http.WebSockets.WebSocketClient;

namespace Queen.Network.Common.Channels;

/// <summary>
/// WebSocketClient 通信渠道
/// </summary>
/// <param name="socket">Socket</param>
public class WebSocketClientC(WSC socket) : NetChannel<WSC>(socket)
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