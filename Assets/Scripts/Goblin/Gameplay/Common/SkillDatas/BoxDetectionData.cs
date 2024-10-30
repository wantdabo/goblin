using Goblin.Gameplay.Common.SkillDatas.Common;
using MessagePack;

namespace Goblin.Gameplay.Common.SkillDatas
{
    /// <summary>
    /// 立方体碰撞检测行为数据
    /// </summary>
    [MessagePackObject(true)]
    public class BoxDetectionData : DetectionData
    {
        /// <summary>
        /// 立方体尺寸
        /// </summary>
        public Vector3Data size { get; set; }
    }
}
