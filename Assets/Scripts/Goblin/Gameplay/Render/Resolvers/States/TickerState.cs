using Goblin.Gameplay.Render.Resolvers.Common;

namespace Goblin.Gameplay.Render.Resolvers.States
{
    /// <summary>
    /// 驱动状态
    /// </summary>
    public struct TickerState : IState
    {
        public StateType type => StateType.Ticker;
        public ulong actor { get; set; }
        public uint frame { get; set; }
        /// <summary>
        /// 时间缩放
        /// </summary>
        public float timescale { get; set; }
    }
}