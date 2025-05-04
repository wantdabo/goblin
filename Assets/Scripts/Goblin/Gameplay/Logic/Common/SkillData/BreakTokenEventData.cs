using Goblin.Gameplay.Common.SkillData.Common;
using MessagePack;

namespace Goblin.Gameplay.Common.SkillData
{
    /// <summary>
    /// 打断标记事件数据
    /// </summary>
    [MessagePackObject(true)]
    public class BreakTokenEventData : SkillActionData
    {
        /// <summary>
        /// 打断标记
        /// </summary>
        public int token { get; set; }
    }
}
