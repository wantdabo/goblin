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
        /// 当前帧驱动了移动, 标记
        /// </summary>
        public bool turnmove { get; set; }
        /// <summary>
        /// 运动类型
        /// </summary>
        public byte motion { get; set; }

        
        protected override void OnReady()
        {
            OnReset();
        }

        protected override void OnReset()
        {
            motion = 0;
            turnmove = false;
        }

        protected override BehaviorInfo OnClone()
        {
            var clone = ObjectCache.Ensure<MovementInfo>();
            clone.Ready(id);
            clone.turnmove = turnmove;
            clone.motion = motion;
            
            return clone;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 31 + id.GetHashCode();
            hash = hash * 31 + turnmove.GetHashCode();
            hash = hash * 31 + motion.GetHashCode();
            
            return hash;
        }
    }
}