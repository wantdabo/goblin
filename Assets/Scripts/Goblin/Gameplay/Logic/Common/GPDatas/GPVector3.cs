namespace Goblin.Gameplay.Logic.Common.GPDatas
{
    /// <summary>
    /// 三维向量数据结构
    /// </summary>
    public struct GPVector3
    {
        /// <summary>
        /// X 轴
        /// </summary>
        public int x { get; set; }
        /// <summary>
        /// Y 轴
        /// </summary>
        public int y { get; set; }
        /// <summary>
        /// Z 轴
        /// </summary>
        public int z { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="x">X 轴</param>
        /// <param name="y">Y 轴</param>
        /// <param name="z">Z 轴</param>
        public GPVector3(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
}