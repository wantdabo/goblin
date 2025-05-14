namespace Goblin.Gameplay.Flows.Executors.Common
{
    /// <summary>
    /// 指令数据
    /// </summary>
    public abstract class InstructData
    {
        /// <summary>
        /// 指令 ID
        /// </summary>
        public abstract ushort id { get; }
    }
}