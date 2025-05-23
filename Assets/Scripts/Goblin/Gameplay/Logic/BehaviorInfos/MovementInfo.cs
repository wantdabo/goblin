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
        /// <summary>
        /// 运动类型
        /// </summary>
        public byte type { get; set; }

        
        protected override void OnReady()
        {
            OnReset();
        }

        protected override void OnReset()
        {
            type = 0;
            turnmotion = false;
        }

        protected override BehaviorInfo OnClone()
        {
            var clone = ObjectCache.Ensure<MovementInfo>();
            clone.Ready(id);
            clone.turnmotion = turnmotion;
            clone.type = type;
            
            return clone;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 31 + id.GetHashCode();
            hash = hash * 31 + turnmotion.GetHashCode();
            hash = hash * 31 + type.GetHashCode();
            
            return hash;
        }
    }
}