using MessagePack;

namespace Queen.Protocols.Common
{
    /// <summary>
    /// RPC ACK 消息
    /// </summary>
    [MessagePackObject(true)]
    public class ACKCrossMessage : INetMessage
    {
        /// <summary>
        /// RPC ID
        /// </summary>
        public string id { get; set; }
    }

    /// <summary>
    /// RPC 发起消息
    /// </summary>
    [MessagePackObject(true)]
    public class ReqCrossMessage : INetMessage
    {
        /// <summary>
        /// RPC ID
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 路径
        /// </summary>
        public string route { get; set; }
        /// <summary>
        /// RPC 内容
        /// </summary>
        public string content { get; set; }
    }
    
    /// <summary>
    /// RPC 响应消息
    /// </summary>
    [MessagePackObject(true)]
    public class ResCrossMessage : INetMessage
    {
        /// <summary>
        /// RPC ID
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// RPC 状态
        /// </summary>
        public ushort state { get; set; }
        /// <summary>
        /// RPC 内容
        /// </summary>
        public string content { get; set; }
    }
}
