using Queen.Network.Common.Channels;
using TouchSocket.Core;
using TouchSocket.Sockets;

namespace Queen.Network.Common;

/// <summary>
/// TCP 服务端网络节点
/// </summary>
public class TCPServer : NetNode
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
    /// 最大工作线程
    /// </summary>
    protected int sthread { get; set; }

    protected override void OnCreate()
    {
        base.OnCreate();
        var service = new TcpService();
        service.Connected = (c, e) =>
        {
            var channel = new TCPServerC(c);
            if (false == AddChannel(channel)) return EasyTask.CompletedTask;
            EmitConnectEvent(channel);

            return EasyTask.CompletedTask;
        };
        service.Closed = (c, e) =>
        {
            if (GetChannel(c.Id, out var channel))
            {
                EmitDisconnectEvent(channel);
                RmvChannel(channel.id);
            }

            return EasyTask.CompletedTask;
        };
        service.Received = (c, e) =>
        {
            if (GetChannel(c.Id, out var channel)) EmitReceiveEvent(channel, e.ByteBlock.Memory.ToArray());

            return EasyTask.CompletedTask;
        };

        service.Setup(new TouchSocketConfig()
            .SetMaxCount(maxconn)
            .SetThreadCount(sthread)
            .SetListenIPHosts(port)
            .SetTcpDataHandlingAdapter(() => new FixedHeaderPackageAdapter())
        );
        service.Start();
    }

    /// <summary>
    /// 创建服务端网络节点
    /// </summary>
    /// <param name="ip">地址</param>
    /// <param name="port">端口</param>
    /// <param name="notify">是否自动通知消息（子线程）</param>
    /// <param name="maxconn">最大连接数</param>
    /// <param name="sthread">最大工作线程</param>
    /// <param name="maxpps">最大网络收发包每秒</param>
    public void Initialize(string ip, ushort port, bool notify, int maxconn, int sthread, int maxpps)
    {
        this.ip = ip;
        this.port = port;
        this.maxconn = maxconn;
        this.sthread = sthread;
        Initialize(notify, maxpps);
    }
}