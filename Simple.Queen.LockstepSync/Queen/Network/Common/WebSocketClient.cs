using Queen.Network.Common.Channels;
using Queen.Protocols.Common;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using TouchSocket.Core;
using TouchSocket.Http.WebSockets;
using TouchSocket.Sockets;

namespace Queen.Network.Common;

/// <summary>
/// WebSocket 客户端
/// </summary>
public class WebSocketClient : NetNode
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
    private WebSocketClientC channel { get; set; }
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

    /// <summary>
    /// 创建服务端网络节点
    /// </summary>
    /// <param name="notify">是否自动通知消息（子线程）</param>
    public void Initialize(bool notify)
    {
        Initialize(notify, int.MaxValue);
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

    /// <summary>
    /// 连接
    /// </summary>
    /// <param name="ip">地址</param>
    /// <param name="port">端口</param>
    public void Connect(string ip, ushort port)
    {
        if (connected) return;

        this.ip = ip;
        this.port = port;
        var client = new TouchSocket.Http.WebSockets.WebSocketClient();
        var config = new TouchSocketConfig();
        if (false == ssl)
        {
            config.SetRemoteIPHost($"ws://{ip}:{port}/ws");
        }
        else
        {
            config.SetRemoteIPHost($"wss://{ip}:{port}/ws");
            config.SetClientSslOption(new ClientSslOption
            {
                ClientCertificates = new X509CertificateCollection() { new X509Certificate2(sslcertname, sslcertpwd) },
                SslProtocols = SslProtocols.Tls12,
                TargetHost = ip,
                CertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => { return true; }
            });
        }

        client.Setup(config);
        channel = new WebSocketClientC(client);
        client.Handshaked = async (c, e) =>
        {
            EmitConnectEvent(channel);
            await e.InvokeNext();
        };
        client.Closed = async (c, e) =>
        {
            EmitDisconnectEvent(channel);
            await e.InvokeNext();
        };
        client.Received = async (c, e) =>
        {
            if (WSDataType.Binary != e.DataFrame.Opcode && false == e.DataFrame.FIN) return;
            EmitReceiveEvent(channel, e.DataFrame.PayloadData.Memory.ToArray());

            await e.InvokeNext();
        };
        client.Connect();
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
