using Goblin.Gameplay.Common.SkillData.Common;
using MessagePack;

namespace Goblin.Gameplay.Common.SkillData
{
    /// <summary>
    /// 球体碰撞检测行为数据
    /// </summary>
    [MessagePackObject(true)]
    public class SphereDetectionData : DetectionData
    {
        /// <summary>
        /// 球体半径
        /// </summary>
        public int radius { get; set; }
    }
}
