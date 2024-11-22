using Kowtow.Math;

namespace Kowtow.Collision.Shapes
{
    /// <summary>
    /// 球体
    /// </summary>
    public class SphereShape : IShape
    {
        /// <summary>
        /// 中心点
        /// </summary>
        public FPVector3 center { get; set; }
        /// <summary>
        /// 半径
        /// </summary>
        public FP radius { get; set; }
        
        /// <summary>
        /// 球体构造函数
        /// </summary>
        /// <param name="center">中心点</param>
        /// <param name="radius">半径</param>
        public SphereShape(FPVector3 center, FP radius)
        {
            this.center = center;
            this.radius = radius;
        }
    }
}
