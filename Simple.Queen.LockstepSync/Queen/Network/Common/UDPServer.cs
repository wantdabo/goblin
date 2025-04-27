using LiteNetLib;
using Queen.Network.Common.Channels;
using Queen.Protocols.Common;
using System.Net;
using TouchSocket.Core;

namespace Queen.Network.Common;

/// <summary>
/// UDP 服务端网络节点
/// </summary>
public class UDPServer : NetNode
{
    /// <summary>
    /// 地址
    /// </summary>
    public string ip { get; protected set; }
    /// <summary>
    /// 端口
    /// </summary>
    public ushort port { get; protected set; }
    /// <summary>
    /// 最大连接数
    /// </summary>
    protected int maxconn { get; set; }
    /// <summary>
    /// 连接校验 KEY
    /// </summary>
    protected string acceptkey { get; set; }
        
    /// <summary>
    /// Socket
    /// </summary>
    private NetManager server { get; set; }
        
    protected override void OnCreate()
    {
        base.OnCreate();
        EventBasedNetListener listener = new EventBasedNetListener();
        server = new NetManager(listener);
        server.Start(port);
        listener.ConnectionRequestEvent += (request) =>
        {
            if (server.ConnectedPeersCount >= maxconn)
            {
                request.Reject();
                return;
            }

            request.AcceptIfKey(acceptkey);
        };

        listener.PeerConnectedEvent += (NetPeer peer) =>
        {
            var channel = new UDPNodeC(peer);
            if (false == AddChannel(channel)) return;
            EmitConnectEvent(channel);
        };

        listener.PeerDisconnectedEvent += (NetPeer peer, DisconnectInfo disconnectInfo) =>
        {
            if (GetChannel(peer.Id.ToString(), out var channel))
            {
                EmitDisconnectEvent(channel);
                RmvChannel(channel.id);
            }
        };

        listener.NetworkReceiveEvent += (NetPeer peer, NetPacketReader reader, byte c, DeliveryMethod deliveryMethod) =>
        {
            if (false == GetChannel(peer.Id.ToString(), out var channel)) return;
            EmitReceiveEvent(channel, reader.GetRemainingBytes());
        };

        // 驱动
        Task.Run(() =>
        {
            var netmanager = server;
            while (netmanager == server && netmanager.IsRunning)
            {
                netmanager.PollEvents();
                Thread.Sleep(1);
            }
        });
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        server.Stop();
    }

    /// <summary>
    /// 创建服务端网络节点
    /// </summary>
    /// <param name="ip">地址</param>
    /// <param name="port">端口</param>
    /// <param name="notify">是否自动通知消息（子线程）</param>
    /// <param name="maxconn">最大连接数</param>
    /// <param name="acceptkey">连接校验 KEY</param>
    /// <param name="maxpps">最大网络收发包每秒</param>
    public void Initialize(string ip, ushort port, bool notify, int maxconn, string acceptkey, int maxpps)
    {
        this.ip = ip;
        this.port = port;
        this.maxconn = maxconn;
        this.acceptkey = acceptkey;
        Initialize(notify, maxpps);
    }
}