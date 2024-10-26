using Goblin.Gameplay.Common.SkillDatas.Action.Common;
using MessagePack;

namespace Goblin.Gameplay.Common.SkillDatas.Action
{
    /// <summary>
    /// 跳帧事件数据
    /// </summary>
    [MessagePackObject(true)]
    public class SkillBreakFramesActionData : SkillActionData
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
