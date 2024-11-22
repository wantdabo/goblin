using Kowtow.Math;

namespace Kowtow.Collision
{
    /// <summary>
    /// 碰撞关系
    /// </summary>
    public struct Collider
    {
        /// <summary>
        /// 刚体
        /// </summary>
        public Rigidbody rigidbody { get; set; }
        /// <summary>
        /// 碰撞点
        /// </summary>
        public FPVector3 point { get; set; }
        /// <summary>
        /// 相对法线
        /// </summary>
        public FPVector3 normal { get; set; }
        /// <summary>
        /// 穿透深度
        /// </summary>
        public FP penetration { get; set; }
    }
}
