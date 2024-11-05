using Goblin.Gameplay.Common.SkillDatas.Common;
using MessagePack;

namespace Goblin.Gameplay.Common.SkillDatas
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
        /// Buff 层数
        /// </summary>
        public uint layer { get; set; }
        /// <summary>
        /// 自身生效
        /// </summary>
        public bool self { get; set; }
        /// <summary>
        /// 命中后生效
        /// </summary>
        public bool hitstamp { get; set; }
    }
}
