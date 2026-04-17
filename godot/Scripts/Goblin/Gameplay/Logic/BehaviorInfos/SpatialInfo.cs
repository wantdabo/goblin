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
        /// <summary>
        /// 上一帧位置, 旋转, 缩放
        /// </summary>
        public (FPVector3 position, FPVector3 euler, FP scale) preframe { get; set; }

        protected override void OnReady()
        {
            OnReset();
        }

        protected override void OnReset()
        {
            position = FPVector3.zero;
            euler = FPVector3.zero;
            scale = FP.One;
            preframe = (FPVector3.zero, FPVector3.zero, FP.One);
        }

        protected override BehaviorInfo OnClone()
        {
            var clone = ObjectCache.Ensure<SpatialInfo>();
            clone.Ready(actor);
            clone.position = position;
            clone.euler = euler;
            clone.scale = scale;
            clone.preframe = preframe;
            
            return clone;
        }
    }
}