using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Behaviors
{
    public class TickerInfo : BehaviorInfo
    {
        /// <summary>
        /// 跳帧
        /// </summary>
        public uint breakframes { get; set; }
        /// <summary>
        /// 帧号
        /// </summary>
        public uint frame { get; set; }
        /// <summary>
        /// 流逝时间
        /// </summary>
        public FP elapsed { get; set; }
        /// <summary>
        /// 时间间隔
        /// </summary>
        public FP tick { get; private set; } = GAME_DEFINE.LOGIC_TICK;

        protected override void OnReady()
        {
            OnReset();
        }

        protected override void OnReset()
        {
            breakframes = 0;
            frame = 0;
            elapsed = FP.Zero;
            tick = GAME_DEFINE.LOGIC_TICK;
        }
    }
}