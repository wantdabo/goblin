using Kowtow.Math;

namespace Goblin.Gameplay.Logic.BehaviorInfos.Collisions
{
    /// <summary>
    /// 球体
    /// </summary>
    public struct Sphere
    {
        /// <summary>
        /// 偏移
        /// </summary>
        public FPVector3 offset { get; set; }
        /// <summary>
        /// 半径
        /// </summary>
        public FP radius { get; set; }
    }
}