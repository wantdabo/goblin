using Queen.Core;
using Queen.Network.Common.Channels;
using Queen.Protocols.Common;
using System.Net.Sockets;
using TouchSocket.Core;
using TouchSocket.Sockets;
using TcpClient = TouchSocket.Sockets.TcpClient;

namespace Queen.Network.Common;

/// <summary>
/// TCP 客户端网络节点
/// </summary>
public class TCPClient : NetNode
{
    /// <summary>
    /// 服务器 IP
    /// </summary>
    public string ip { get; private set; }
    /// <summary>
    /// 服务器端口
    /// </summary>
    public ushort port { get; private set; }
    /// <summary>
    /// 连接状态
    /// </summary>
    public bool connected => null != channel && channel.alive;
    /// <summary>
    /// 通信渠道
    /// </summary>
    private TCPClientC channel { get; set; }

    /// <summary>
    /// 创建服务端网络节点
    /// </summary>
    /// <param name="notify">是否自动通知消息（子线程）</param>
    public void Initialize(bool notify)
    {
        Initialize(notify, int.MaxValue);
    }

    /// <summary>
    /// 连接
    /// </summary>
    /// <param name="ip">地址</param>
    /// <param name="port">端口</param>
    public void Connect(string ip, ushort port)
    {
        try
        {
            if (connected) return;
            this.ip = ip;
            this.port = port;

            var tcpClient = new TcpClient();
            channel = new TCPClientC(tcpClient);
            tcpClient.Connected = (c, e) =>
            {
                EmitConnectEvent(channel);
                return EasyTask.CompletedTask;
            };
            tcpClient.Closed = (c, e) =>
            {
                EmitDisconnectEvent(channel);
                return EasyTask.CompletedTask;
            };
            tcpClient.Received = (c, e) =>
            {
                EmitReceiveEvent(channel, e.ByteBlock.Memory.ToArray());
                return EasyTask.CompletedTask;
            };
            tcpClient.Setup(new TouchSocketConfig()
                .SetRemoteIPHost($"{ip}:{port}")
                .SetTcpDataHandlingAdapter(() => new FixedHeaderPackageAdapter())
            );
            tcpClient.Connect();
        }
        catch (SocketException e)
        {
            
        }
    }

    /// <summary>
    /// 断开连接
    /// </summary>
    public void Disconnect()
    {
        channel.Disconnect();
    }

    /// <summary>
    /// 发送数据
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="msg">数据</param>
    public void Send<T>(T msg) where T : INetMessage
    {
        channel.Send(msg);
    }

    /// <summary>
    /// 发送数据
    /// </summary>
    /// <param name="data">二进制数据</param>
    public void Send(byte[] data)
    {
        channel.Send(data);
    }
}
