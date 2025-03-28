using MessagePack;

namespace Goblin.Gameplay.Common.SkillData
{
    /// <summary>
    /// 技能管线数据
    /// </summary>
    [MessagePackObject(true)]
    public class SkillPipelineData
    {
        /// <summary>
        /// 技能 ID
        /// </summary>
        public uint id { get; set; }
        /// <summary>
        /// 帧数
        /// </summary>
        public uint length { get; set; }
        /// <summary>
        /// 技能行为 ID
        /// </summary>
        public ushort[] actionIds { get; set; }
        /// <summary>
        /// 技能行为数据
        /// </summary>
        public byte[][] actionBytes { get; set; }
    }
}
