using Kowtow.Math;

namespace Kowtow.Collision.Shapes
{
    /// <summary>
    /// 圆柱体
    /// </summary>
    public class CylinderShape : IShape
    {
        /// <summary>
        /// 中心点
        /// </summary>
        public FPVector3 center { get; set; }
        
        /// <summary>
        /// 高度
        /// </summary>
        public FP height { get; set; }
        /// <summary>
        /// 半径
        /// </summary>
        public FP radius { get; set; }
        
        /// <summary>
        /// 圆柱体构造函数
        /// </summary>
        /// <param name="center">中心点</param>
        /// <param name="height">高度</param>
        /// <param name="radius">半径</param>
        public CylinderShape(FPVector3 center, FP height, FP radius)
        {
            this.center = center;
            this.height = height;
            this.radius = radius;
        }
    }
}
