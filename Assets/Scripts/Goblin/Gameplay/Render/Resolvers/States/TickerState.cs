using Goblin.Gameplay.Render.Resolvers.Common;

namespace Goblin.Gameplay.Render.Resolvers.States
{
    /// <summary>
    /// 驱动状态
    /// </summary>
    public class TickerState : State
    {
        public override StateType type => StateType.Ticker;
        /// <summary>
        /// 时间缩放
        /// </summary>
        public float timescale { get; set; }
        
        protected override void OnReset()
        {
            timescale = 1.0f;
        }
    }
}