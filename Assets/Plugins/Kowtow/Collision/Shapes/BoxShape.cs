using Kowtow.Math;

namespace Kowtow.Collision.Shapes
{
    /// <summary>
    /// 立方体
    /// </summary>
    public class BoxShape : IShape
    {
        /// <summary>
        /// 中心点
        /// </summary>
        public FPVector3 center { get; set; }

        /// <summary>
        /// 尺寸
        /// </summary>
        public FPVector3 size { get; set; }

        /// <summary>
        /// 立方体构造函数
        /// </summary>
        /// <param name="center">中心点</param>
        /// <param name="size">尺寸</param>
        public BoxShape(FPVector3 center, FPVector3 size)
        {
            this.center = center;
            this.size = size;
        }
    }
}
