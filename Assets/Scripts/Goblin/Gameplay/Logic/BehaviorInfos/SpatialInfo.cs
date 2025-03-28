using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.BehaviorInfos
{
    public class SpatialInfo : IBehaviorInfo
    {
        /// <summary>
        /// 平移
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

        public void OnReady()
        {
            OnReset();
        }

        public void OnReset()
        {
            position = FPVector3.zero;
            euler = FPVector3.zero;
            scale = FPVector3.one;
        }
    }
}