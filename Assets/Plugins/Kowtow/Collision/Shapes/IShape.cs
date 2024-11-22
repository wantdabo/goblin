using Kowtow.Math;

namespace Kowtow.Collision.Shapes
{
    /// <summary>
    /// 几何体
    /// </summary>
    public interface IShape
    {
        /// <summary>
        /// 中心点
        /// </summary>
        public FPVector3 center { get; set; }
    }
}
