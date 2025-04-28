namespace Goblin.Gameplay.Logic.Common.GPDatas
{
    /// <summary>
    /// 游戏数据
    /// </summary>
    public struct GPData
    {
        /// <summary>
        /// GameID
        /// </summary>
        public ulong id { get; set; }
        /// <summary>
        /// 我的座位
        /// </summary>
        public ulong seat { get; set; }
        /// <summary>
        /// Stage 数据
        /// </summary>
        public GPStageData sdata { get; set; }
    }
}