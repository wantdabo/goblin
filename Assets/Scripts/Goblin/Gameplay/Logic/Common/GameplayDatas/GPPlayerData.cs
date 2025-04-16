namespace Goblin.Gameplay.Logic.Common.GameplayDatas
{
    /// <summary>
    /// 玩家数据
    /// </summary>
    public class GPPlayerData
    {
        /// <summary>
        /// 座位 ID
        /// </summary>
        public ulong seat { get; set; }
        /// <summary>
        /// 英雄 ID
        /// </summary>
        public int hero { get; set; }
        /// <summary>
        /// 位置
        /// </summary>
        public GPVector3 position { get; set; }
        /// <summary>
        /// 旋转
        /// </summary>
        public GPVector3 euler { get; set; }
        /// <summary>
        /// 缩放
        /// </summary>
        public GPVector3 scale { get; set; }
    }
}