using System;
using MessagePack;

namespace Kowtow.Math
{
    /// <summary>
    /// 三维向量数据结构
    /// </summary>
    [Serializable]
    [MessagePackObject(true)]
    public struct IntVector3
    {
        /// <summary>
        /// X 轴
        /// </summary>
        public int x;

        /// <summary>
        /// Y 轴
        /// </summary>
        public int y;

        /// <summary>
        /// Z 轴
        /// </summary>
        public int z;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="x">X 轴</param>
        /// <param name="y">Y 轴</param>
        /// <param name="z">Z 轴</param>
        public IntVector3(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
}