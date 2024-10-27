using Goblin.Gameplay.Common.SkillDatas.Common;
using MessagePack;

namespace Goblin.Gameplay.Common.SkillDatas
{
    /// <summary>
    /// 打断标记事件数据
    /// </summary>
    [MessagePackObject(true)]
    public class BreakEventData : SkillActionData
    {
        /// <summary>
        /// 打断标记
        /// </summary>
        public int token { get; set; }
    }
}
