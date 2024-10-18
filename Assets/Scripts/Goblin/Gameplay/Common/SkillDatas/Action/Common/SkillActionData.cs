using MessagePack;

namespace Goblin.Gameplay.Common.SkillDatas.Action.Common
{
    /// <summary>
    /// 技能行为数据
    /// </summary>
    [MessagePackObject(true)]
    public abstract class SkillActionData
    {
        /// <summary>
        /// 技能行为 ID
        /// </summary>
        public ushort id { set; get; }
        /// <summary>
        /// 起始帧号
        /// </summary>
        public uint sframe { get; set; }
        /// <summary>
        /// 结束帧号
        /// </summary>
        public uint eframe { get; set; }
    }
}
