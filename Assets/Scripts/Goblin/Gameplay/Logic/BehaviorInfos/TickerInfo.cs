using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.BehaviorInfos
{
    /// <summary>
    /// 驱动信息
    /// </summary>
    public class TickerInfo : BehaviorInfo
    {
        /// <summary>
        /// 帧号
        /// </summary>
        public uint frame { get; set; }
        /// <summary>
        /// 时间缩放
        /// </summary>
        public FP timescale { get; set; }

        protected override void OnReady()
        {
            OnReset();
        }

        protected override void OnReset()
        {
            frame = 0;
            timescale = FP.One;
        }

        protected override BehaviorInfo OnClone()
        {
            var clone = ObjectCache.Get<TickerInfo>();
            clone.Ready(id);
            clone.frame = frame;
            clone.timescale = timescale;
            
            return clone;
        }
    }
}