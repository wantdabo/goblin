namespace Goblin.Gameplay.Logic.Common.GPDatas
{
    /// <summary>
    /// Stage 数据
    /// </summary>
    public struct GPStageData
    {
        /// <summary>
        /// 游戏的种子，用于随机数生成等目的
        /// </summary>
        public int seed { get; set; }
        /// <summary>
        /// 玩家数据数组，包含了所有参与游戏的玩家信息
        /// </summary>
        public GPPlayerData[] players { get; set; }
    }
}