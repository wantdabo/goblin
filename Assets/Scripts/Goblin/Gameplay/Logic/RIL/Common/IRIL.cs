using Goblin.Common;

namespace Goblin.Gameplay.Logic.RIL.Common
{
    /// <summary>
    /// 渲染指令事件
    /// </summary>
    public struct RILSyncEvent : IEvent
    {
        /// <summary>
        /// ActorID
        /// </summary>
        public uint id { get; set; }
        /// <summary>
        /// 帧号
        /// </summary>
        public uint frame { get; set; }
        /// <summary>
        /// 渲染指令
        /// </summary>
        public IRIL ril { get; set; }
    }
    
    /// <summary>
    /// 渲染指令接口
    /// </summary>
    public interface IRIL
    {
        /// <summary>
        /// 指令
        /// </summary>
        public ushort id { get; }
        /// <summary>
        /// 序列化
        /// </summary>
        /// <returns>二进制</returns>
        public byte[] Serialize();
        /// <summary>
        /// 对比指令
        /// </summary>
        /// <param name="other">目标指令</param>
        /// <returns>YES/NO</returns>
        public bool Equals(IRIL other);
    }
}
