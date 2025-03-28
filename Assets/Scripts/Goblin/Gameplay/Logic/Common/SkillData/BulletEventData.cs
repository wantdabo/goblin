using Goblin.Gameplay.Common.SkillData.Common;
using MessagePack;

namespace Goblin.Gameplay.Common.SkillData
{
    /// <summary>
    /// 打断标记事件数据
    /// </summary>
    [MessagePackObject(true)]
    public class BulletEventData : SkillActionData
    {
        /// <summary>
        /// 子弹 ID
        /// </summary>
        public uint bulletid { get; set; }
        /// <summary>
        /// 起始位置
        /// </summary>
        public Vector3Data position { get; set; }
    }
}
