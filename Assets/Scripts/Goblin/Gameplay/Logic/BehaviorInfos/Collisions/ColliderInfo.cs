using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.BehaviorInfos.Collisions
{
    /// <summary>
    /// 碰撞盒
    /// </summary>
    public class ColliderInfo : BehaviorInfo
    {
        /// <summary>
        /// 几何体类型
        /// </summary>
        public byte shape { get; set; }
        /// <summary>
        /// 偏移
        /// </summary>
        public FPVector3 offset { get; set; }
        /// <summary>
        /// 尺寸
        /// </summary>
        public FPVector3 size { get; set; }
        /// <summary>
        /// 半径
        /// </summary>
        public FP radius { get; set; }
        
        protected override void OnReady()
        {
            OnReset();
        }

        protected override void OnReset()
        {
            shape = 0;
            offset = FPVector3.zero;
            size = FPVector3.zero;
            radius = FP.Zero;
        }

        protected override BehaviorInfo OnClone()
        {
            var clone = ObjectCache.Get<ColliderInfo>();
            clone.Ready(id);
            clone.shape = shape;
            clone.offset = offset;
            clone.size = size;
            clone.radius = radius;

            return clone;
        }
    }
}