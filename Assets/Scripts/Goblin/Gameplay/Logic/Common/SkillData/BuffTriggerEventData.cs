using Goblin.Gameplay.Common.SkillData.Common;
using MessagePack;

namespace Goblin.Gameplay.Common.SkillData
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
        /// <summary>
        /// 自身生效
        /// </summary>
        public int triggerself { get; set; }
        /// <summary>
        /// 目标生效
        /// </summary>
        public int triggertarget { get; set; }
    }
}
