using TouchSocket.Http;
using TouchSocket.Sockets;

namespace Queen.Network.Common.Channels
{
    /// <summary>
    /// WebSocketServ 通信渠道
    /// </summary>
    /// <param name="socket">Socket</param>
    public class WebSocketServerC(HttpSessionClient socket) : NetChannel<HttpSessionClient>(socket)
    {
        public override string id => socket.Id;
        public override bool alive => socket.Online;
        
        public override void Send(byte[] data)
        {
            socket.WebSocket.SendAsync(data);
        }
        
        public override void Disconnect()
        {
            socket.Close();
        }
    }
}
