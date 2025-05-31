using MessagePack;

namespace Goblin.Gameplay.Logic.Flows.Checkers.Common
{
    /// <summary>
    /// 管线指令执行条件
    /// </summary>
    [MessagePackObject(true)]
    public abstract class Condition
    {
        /// <summary>
        /// 条件 ID
        /// </summary>
        public abstract ushort id { get; }
    }
}