using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.BehaviorInfos
{
    public class TickerInfo : IBehaviorInfo
    {
        /// <summary>
        /// 时间缩放
        /// </summary>
        public FP timescale { get; set; } = FP.One;

        public void Ready()
        {
            Reset();
        }

        public void Reset()
        {
            timescale = FP.One;
        }
    }
}