using Goblin.Gameplay.Common.SkillData.Common;
using MessagePack;

namespace Goblin.Gameplay.Common.SkillData
{
    /// <summary>
    /// 空间行为数据
    /// </summary>
    [MessagePackObject(true)]
    public class SpatialData : SkillActionData
    {
        /// <summary>
        /// 平移
        /// </summary>
        public Vector3Data position { get; set; }
        /// <summary>
        /// 缩放
        /// </summary>
        public int scale { get; set; }
    }
}
