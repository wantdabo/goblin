namespace Goblin.Gameplay.Logic.RIL.Common
{
    /// <summary>
    /// RIL 渲染状态
    /// </summary>
    public struct RILState
    {
        /// <summary>
        /// 序号
        /// </summary>
        public uint index { get; set; }
        /// <summary>
        /// 帧号
        /// </summary>
        public uint frame { get; set; }
        /// <summary>
        /// ActorID
        /// </summary>
        public ulong actor { get; set; }
        /// <summary>
        /// RIL 渲染指令
        /// </summary>
        public IRIL ril { get; set; }
    }
}