using Goblin.Gameplay.Common.SkillDatas.Common;
using MessagePack;

namespace Goblin.Gameplay.Common.SkillDatas
{
    /// <summary>
    /// Buff 触发事件数据
    /// </summary>
    [MessagePackObject(true)]
    public class BuffTriggerEventData : SkillActionData
    {
        /// <summary>
        /// BuffID
        /// </summary>
        public uint buffid { get; set; }
    }
}
