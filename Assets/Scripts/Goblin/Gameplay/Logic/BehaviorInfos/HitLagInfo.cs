using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.BehaviorInfos
{
    /// <summary>
    /// 顿帧信息
    /// </summary>
    public class HitLagInfo : BehaviorInfo
    {
        /// <summary>
        /// 修改前的时间缩放
        /// </summary>
        public FP timescale { get; set; }
        /// <summary>
        /// 强度
        /// </summary>
        public FP strength { get; set; }
        /// <summary>
        /// 持续时间
        /// </summary>
        public FP duration { get; set; }
        /// <summary>
        /// 已流逝时间
        /// </summary>
        public FP elapsed { get; set; }
        
        protected override void OnReady()
        {
            timescale = FP.Zero;
            strength = FP.Zero;
            duration = FP.Zero;
            elapsed = FP.Zero;
        }

        protected override void OnReset()
        {
            timescale = FP.Zero;
            strength = FP.Zero;
            duration = FP.Zero;
            elapsed = FP.Zero;
        }

        protected override BehaviorInfo OnClone()
        {
            var clone = ObjectCache.Ensure<HitLagInfo>();
            clone.Ready(actor);
            clone.timescale = timescale;
            clone.strength = strength;
            clone.duration = duration;
            clone.elapsed = elapsed;

            return clone;
        }
    }
}