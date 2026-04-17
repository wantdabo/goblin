using MessagePack;
using Queen.Protocols.Common;

namespace Queen.Protocols
{
    /// <summary>
    /// 心跳
    /// </summary>
    [MessagePackObject(true)]
    public class S2CHeartbeatMsg : INetMessage
    {
        /// <summary>
        /// 时间戳
        /// </summary>
        public long timestamp { get; set; }
    }
}