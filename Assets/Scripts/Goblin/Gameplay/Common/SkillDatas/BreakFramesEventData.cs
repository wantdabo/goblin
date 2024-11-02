using Goblin.Gameplay.Common.SkillDatas.Common;
using MessagePack;

namespace Goblin.Gameplay.Common.SkillDatas
{
    /// <summary>
    /// 跳帧事件数据
    /// </summary>
    [MessagePackObject(true)]
    public class BreakFramesEventData : SkillActionData
    {
        /// <summary>
        /// 自身跳帧
        /// </summary>
        public uint selfbreakframes { get; set; }
        /// <summary>
        /// 目标跳帧
        /// </summary>
        public uint targetbreakframes { get; set; }
    }
}
