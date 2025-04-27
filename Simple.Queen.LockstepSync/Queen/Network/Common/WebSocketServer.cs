using Queen.Network.Common.Channels;
using System.Collections.Concurrent;
using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using TouchSocket.Core;
using TouchSocket.Http;
using TouchSocket.Http.WebSockets;
using TouchSocket.Sockets;

namespace Queen.Network.Common;

/// <summary>
/// WebSocket 插件
/// </summary>
public class WebSocketPlugin : PluginBase, IWebSocketHandshakedPlugin, IWebSocketClosedPlugin, IWebSocketReceivedPlugin
{
    public event Action<HttpSessionClient> OnConnect;
    public event Action<HttpSessionClient> OnDisconnect;
    public event Action<HttpSessionClient, byte[]> OnReceive;

    public async Task OnWebSocketHandshaked(IWebSocket client, HttpContextEventArgs e)
    {
        HttpSessionClient socket = (HttpSessionClient)client.Client;
        OnConnect(socket);
        await e.InvokeNext();
    }
    public async Task OnWebSocketClosed(IWebSocket client, ClosedEventArgs e)
    {
        HttpSessionClient socket = (HttpSessionClient)client.Client;
        OnDisconnect(socket);
        await e.InvokeNext();
    }

    public async Task OnWebSocketReceived(IWebSocket client, WSDataFrameEventArgs e)
    {
        if (WSDataType.Binary != e.DataFrame.Opcode && false == e.DataFrame.FIN) return;
        HttpSessionClient socket = (HttpSessionClient)client.Client;
        OnReceive(socket, e.DataFrame.PayloadData.Memory.ToArray());
        await e.InvokeNext();
    }
}

/// <summary>
/// WebSocket 服务端
/// </summary>
public class WebSocketServer : NetNode
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
    /// <summary>
    /// SSL 开关
    /// </summary>
    public bool ssl { get; protected set; } = false;
    /// <summary>
    /// SSLCert 文件名
    /// </summary>
    public string sslcertname { get; protected set; }
    /// <summary>
    /// SSLCert 密码
    /// </summary>
    public string sslcertpwd { get; protected set; }

    protected override void OnCreate()
    {
        base.OnCreate();
        var service = new HttpService();
        var config = new TouchSocketConfig()
            .SetListenIPHosts(new IPHost(IPAddress.Parse(ip), port))
            .ConfigurePlugins(a =>
                {
                    a.UseWebSocket().SetWSUrl("/ws").UseAutoPong();
                    a.Add<WebSocketPlugin>();
                }
            )
            .SetMaxCount(maxconn)
            .SetThreadCount(sthread);

        if (ssl)
        {
            config.SetServiceSslOption(new ServiceSslOption
            {
                Certificate = new X509Certificate2(sslcertname, sslcertpwd),
                SslProtocols = SslProtocols.Tls12
            });
        }

        service.Setup(config);
        foreach (var plugin in service.PluginManager.Plugins)
        {
            if (plugin is WebSocketPlugin wsplugin)
            {
                wsplugin.OnConnect += OnConnect;
                wsplugin.OnDisconnect += OnDisconnect;
                wsplugin.OnReceive += OnReceive;
                break;
            }
        }

        service.StartAsync();
    }

    private void OnConnect(HttpSessionClient socket)
    {
        var channel = new WebSocketServerC(socket);
        if (false == AddChannel(channel)) return;
        EmitConnectEvent(channel);
    }

    private void OnDisconnect(HttpSessionClient socket)
    {
        if (GetChannel(socket.Id, out var channel))
        {
            EmitDisconnectEvent(channel);
            RmvChannel(channel.id);
        }
    }

    private void OnReceive(HttpSessionClient socket, byte[] data)
    {
        if (GetChannel(socket.Id, out var channel)) EmitReceiveEvent(channel, data);
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

    /// <summary>
    /// 开启 SSL/WSS
    /// </summary>
    /// <param name="sslcertname">SSLCert 文件名</param>
    /// <param name="sslcertpwd">SSLCert 密码</param>
    public void EnabledSSL(string sslcertname, string sslcertpwd)
    {
        ssl = true;
        this.sslcertname = sslcertname;
        this.sslcertpwd = sslcertpwd;
    }
}
