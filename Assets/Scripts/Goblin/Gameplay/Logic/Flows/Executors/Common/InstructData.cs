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
    }
}