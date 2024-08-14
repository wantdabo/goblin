using MessagePack;
using Queen.Protocols.Common;

namespace Queen.Protocols
{
    /// <summary>
    /// 请求登出消息
    /// </summary>
    [MessagePackObject(true)]
    public class C2STestMsg : INetMessage
    {
        /// <summary>
        /// 内容
        /// </summary>
        public string content { get; set; }
    }
    
    /// <summary>
    /// 请求登出消息
    /// </summary>
    [MessagePackObject(true)]
    public class S2CTestMsg : INetMessage
    {
        /// <summary>
        /// 内容
        /// </summary>
        public string content { get; set; }
    }
}
