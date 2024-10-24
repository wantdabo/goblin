using Goblin.Gameplay.Common.SkillDatas.Action.Common;
using MessagePack;

namespace Goblin.Gameplay.Common.SkillDatas.Action
{
    /// <summary>
    /// 球体碰撞检测行为数据
    /// </summary>
    [MessagePackObject(true)]
    public class SphereDetectionActionData : DetectionActionData
    {
        /// <summary>
        /// 球体半径
        /// </summary>
        public int radius { get; set; }
    }
}
