using System;
using MessagePack;

namespace Kowtow.Math
{
    /// <summary>
    /// 二维向量数据结构
    /// </summary>
    [Serializable]
    [MessagePackObject(true)]
    public struct IntVector2
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
        /// 构造函数
        /// </summary>
        /// <param name="x">X 轴</param>
        /// <param name="y">Y 轴</param>
        public IntVector2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}