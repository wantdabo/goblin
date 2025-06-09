using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Common.Collisions
{
    /// <summary>
    /// 包围盒
    /// </summary>
    public struct AABB
    {
        /// <summary>
        /// 位置
        /// </summary>
        public FPVector3 position { get; set; }
        /// <summary>
        /// 尺寸
        /// </summary>
        public FPVector3 size { get; set; }
    }
}