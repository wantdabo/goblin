using Goblin.Gameplay.Common.SkillDatas.Action.Common;
using MessagePack;

namespace Goblin.Gameplay.Common.SkillDatas.Action
{
    /// <summary>
    /// 打断标记事件数据
    /// </summary>
    [MessagePackObject(true)]
    public class BreakEventActionData : SkillActionData
    {
        /// <summary>
        /// 打断标记
        /// </summary>
        public int token { get; set; }
    }
}
