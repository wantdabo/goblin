using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Prefabs.Datas
{
    /// <summary>
    /// 空间数据结构
    /// </summary>
    public struct SpatialData
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
    }
}