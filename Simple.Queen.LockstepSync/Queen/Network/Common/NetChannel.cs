using Queen.Protocols.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouchSocket.Sockets;

namespace Queen.Network.Common;

/// <summary>
/// 通信渠道
/// </summary>
public abstract class NetChannel
{
    /// <summary>
    /// ID
    /// </summary>
    public abstract string id { get; }

    /// <summary>
    /// 活性
    /// </summary>
    public abstract bool alive { get; }

    /// <summary>
    /// 发送数据
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="msg">数据</param>
    public void Send<T>(T msg) where T : INetMessage
    {
        if (ProtoPack.Pack(msg, out var bytes)) Send(bytes);
    }

    /// <summary>
    /// 发送数据
    /// </summary>
    /// <param name="data">二进制数据</param>
    public abstract void Send(byte[] data);

    /// <summary>
    /// 断开连接
    /// </summary>
    public abstract void Disconnect();
}
    
/// <summary>
/// 通信渠道
/// </summary>
/// <typeparam name="T">通信节点</typeparam>
public abstract class NetChannel<T> : NetChannel
{
    /// <summary>
    /// Node
    /// </summary>
    protected T socket { get; private set; }

    public NetChannel(T socket)
    {
        this.socket = socket;
    }
}