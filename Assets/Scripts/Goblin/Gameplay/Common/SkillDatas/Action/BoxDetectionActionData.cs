using Goblin.Gameplay.Common.SkillDatas.Action.Common;
using MessagePack;

namespace Goblin.Gameplay.Common.SkillDatas.Action
{
    /// <summary>
    /// 立方体碰撞检测行为数据
    /// </summary>
    [MessagePackObject(true)]
    public class BoxDetectionActionData : DetectionActionData
    {
        /// <summary>
        /// 立方体尺寸
        /// </summary>
        public Vector3Data size { get; set; }
    }
}
