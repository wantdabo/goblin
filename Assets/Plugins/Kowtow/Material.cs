using Kowtow.Math;

namespace Kowtow
{
    /// <summary>
    /// 物理材质
    /// </summary>
    public struct Material
    {
        /// <summary>
        /// 摩檫力
        /// </summary>
        public FP friction { get; set; }
        /// <summary>
        /// 弹性
        /// </summary>
        public FP bounciness { get; set; }
        
        /// <summary>
        /// 物理材质构造函数
        /// </summary>
        /// <param name="friction">摩檫力</param>
        /// <param name="bounciness">弹力</param>
        public Material(FP friction, FP bounciness)
        {
            this.friction = friction;
            this.bounciness = bounciness;
        }
    }
}
