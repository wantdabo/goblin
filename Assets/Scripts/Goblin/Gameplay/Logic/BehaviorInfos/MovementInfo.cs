using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.BehaviorInfos
{
    /// <summary>
    /// 运动信息
    /// </summary>
    public class MovementInfo : BehaviorInfo
    {
        /// <summary>
        /// 当前帧驱动了运动, 标记
        /// </summary>
        public bool turnmotion { get; set; }

        protected override void OnReady()
        {
            OnReset();
        }

        protected override void OnReset()
        {
            turnmotion = false;
        }

        protected override BehaviorInfo OnClone()
        {
            var clone = ObjectCache.Ensure<MovementInfo>();
            clone.Ready(actor);
            clone.turnmotion = turnmotion;
            
            return clone;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 31 + actor.GetHashCode();
            hash = hash * 31 + turnmotion.GetHashCode();
            
            return hash;
        }
    }
}