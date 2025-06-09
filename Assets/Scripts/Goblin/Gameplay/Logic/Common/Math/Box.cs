using Kowtow.Math;

namespace Kowtow.Math
{
    /// <summary>
    /// 立方体
    /// </summary>
    public struct Box
    {
        /// <summary>
        /// 偏移
        /// </summary>
        public FPVector3 offset { get; set; }
        /// <summary>
        /// 尺寸
        /// </summary>
        public FPVector3 size { get; set; }
    }
}