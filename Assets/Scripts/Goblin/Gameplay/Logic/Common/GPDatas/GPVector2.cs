namespace Goblin.Gameplay.Logic.Common.GPDatas
{
    /// <summary>
    /// 二维向量数据结构
    /// </summary>
    public struct GPVector2
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
        /// 构造函数
        /// </summary>
        /// <param name="x">X 轴</param>
        /// <param name="y">Y 轴</param>
        public GPVector2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}