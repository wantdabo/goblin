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
        public FPVector3 scale { get; set; }

        protected override void OnReady()
        {
            OnReset();
        }

        protected override void OnReset()
        {
            position = FPVector3.zero;
            euler = FPVector3.zero;
            scale = FPVector3.one;
        }

        protected override BehaviorInfo OnClone()
        {
            var clone = ObjectCache.Get<SpatialInfo>();
            clone.Ready(id);
            clone.position = position;
            clone.euler = euler;
            clone.scale = scale;
            
            return clone;
        }
    }
}