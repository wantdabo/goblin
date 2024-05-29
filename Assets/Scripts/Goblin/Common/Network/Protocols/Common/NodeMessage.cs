using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Queen.Protocols.Common
{
    /// <summary>
    /// 网络节点连接消息
    /// </summary>
    public class NodeConnectMsg : INetMessage { }

    /// <summary>
    /// 网络节点断开连接消息
    /// </summary>
    public class NodeDisconnectMsg : INetMessage { }

    /// <summary>
    /// 网络节点超时消息
    /// </summary>
    public class NodeTimeoutMsg : INetMessage { }

    /// <summary>
    /// 网络节点接收消息
    /// </summary>
    public class NodeReceiveMsg : INetMessage
    {
        public byte[] data;
    }

    /// <summary>
    /// 网络节点延迟
    /// </summary>
    public class NodePingMsg : INetMessage { }
}