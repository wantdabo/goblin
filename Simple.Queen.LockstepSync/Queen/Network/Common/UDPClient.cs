using LiteNetLib;
using Queen.Network.Common.Channels;
using Queen.Protocols.Common;

namespace Queen.Network.Common;

/// <summary>
/// UDP 客户端网络节点
/// </summary>
public class UDPClient : NetNode
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
    /// 连接校验 KEY
    /// </summary>
    protected string connectkey { get; set; }
    /// <summary>
    /// 连接状态
    /// </summary>
    public bool connected => null != channel && channel.alive;

    /// <summary>
    /// 通信渠道
    /// </summary>
    private UDPNodeC channel { get; set; }

    /// <summary>
    /// Socket
    /// </summary>
    private NetManager client { get; set; }

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
    /// <param name="connectkey">连接校验 KEY</param>
    public void Connect(string ip, ushort port, string connectkey)
    {
        this.connectkey = connectkey;
        if (connected) { Disconnect(); }
        EventBasedNetListener listener = new EventBasedNetListener();
        client = new NetManager(listener);
        client.Start();
        listener.PeerConnectedEvent += (NetPeer peer) =>
        {
            EmitConnectEvent(channel);
        };

        listener.PeerDisconnectedEvent += (NetPeer peer, DisconnectInfo disconnectInfo) =>
        {
            EmitDisconnectEvent(channel);
        };

        listener.NetworkReceiveEvent += (NetPeer peer, NetPacketReader reader, byte c, DeliveryMethod deliveryMethod) =>
        {
            EmitReceiveEvent(channel, reader.GetRemainingBytes());
        };
        channel = new UDPNodeC(client.Connect(ip, port, connectkey));
            
        // 驱动
        Task.Run(() =>
        {
            var netmanager = client;
            while (netmanager == client && netmanager.IsRunning)
            {
                netmanager.PollEvents();
                Thread.Sleep(1);
            }
        });
    }

    /// <summary>
    /// 断开连接
    /// </summary>
    public void Disconnect()
    {
        channel.Disconnect();
        client.Stop();
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