using MessagePack;

namespace Goblin.Gameplay.Logic.Flows.Executors.Common
{
    /// <summary>
    /// 指令数据
    /// </summary>
    [MessagePackObject(true)]
    public abstract class InstructData
    {
        /// <summary>
        /// 指令 ID
        /// </summary>
        public abstract ushort id { get; }
        /// <summary>
        /// 序列化指令数据
        /// </summary>
        /// <returns>二进制数据</returns>
        public abstract byte[] Serialize();
    }
}