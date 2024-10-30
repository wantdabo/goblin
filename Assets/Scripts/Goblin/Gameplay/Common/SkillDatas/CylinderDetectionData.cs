using Goblin.Gameplay.Common.SkillDatas.Common;
using MessagePack;

namespace Goblin.Gameplay.Common.SkillDatas
{
    /// <summary>
    /// 圆柱体碰撞检测行为数据
    /// </summary>
    [MessagePackObject(true)]
    public class CylinderDetectionData : DetectionData
    {
        /// <summary>
        /// 圆柱体半径
        /// </summary>
        public int radius { get; set; }
        /// <summary>
        /// 圆柱体高度
        /// </summary>
        public int height { get; set; }
    }
}
