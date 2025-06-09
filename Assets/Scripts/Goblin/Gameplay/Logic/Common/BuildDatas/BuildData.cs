using MessagePack;

namespace Goblin.Gameplay.Logic.Common.BuildDatas
{
    /// <summary>
    /// 游戏数据
    /// </summary>
    [MessagePackObject(true)]
    public struct BuildData
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
        public StageData sdata { get; set; }
    }
}