using Goblin.Gameplay.Common.SkillData.Common;
using MessagePack;

namespace Goblin.Gameplay.Common.SkillData
{
    /// <summary>
    /// Buff 印下事件数据
    /// </summary>
    [MessagePackObject(true)]
    public class BuffStampEventData : SkillActionData
    {
        /// <summary>
        /// BuffID
        /// </summary>
        public uint buffid { get; set; }
        /// <summary>
        /// 自身生效
        /// </summary>
        public int stampself { get; set; }
        /// <summary>
        /// 目标生效
        /// </summary>
        public int stamptarget { get; set; }
        /// <summary>
        /// Buff 层数
        /// </summary>
        public uint layer { get; set; }
    }
}
