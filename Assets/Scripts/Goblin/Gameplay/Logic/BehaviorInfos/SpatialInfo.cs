using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.BehaviorInfos
{
    /// <summary>
    /// 空间信息
    /// </summary>
    public class SpatialInfo : BehaviorInfo
    {
        /// <summary>
        /// 位置
        /// </summary>
        public FPVector3 position { get; set; }
        /// <summary>
        /// 旋转
        /// </summary>
        public FPVector3 euler { get; set; }
        /// <summary>
        /// 缩放
        /// </summary>
        public FP scale { get; set; }

        protected override void OnReady()
        {
            OnReset();
        }

        protected override void OnReset()
        {
            position = FPVector3.zero;
            euler = FPVector3.zero;
            scale = FP.One;
        }

        protected override BehaviorInfo OnClone()
        {
            var clone = ObjectCache.Ensure<SpatialInfo>();
            clone.Ready(actor);
            clone.position = position;
            clone.euler = euler;
            clone.scale = scale;
            
            return clone;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 31 + actor.GetHashCode();
            hash = hash * 31 + position.x.GetHashCode();
            hash = hash * 31 + position.y.GetHashCode();
            hash = hash * 31 + position.z.GetHashCode();
            hash = hash * 31 + euler.x.GetHashCode();
            hash = hash * 31 + euler.y.GetHashCode();
            hash = hash * 31 + euler.z.GetHashCode();
            hash = hash * 31 + scale.GetHashCode();
            
            return hash;
        }
    }
}