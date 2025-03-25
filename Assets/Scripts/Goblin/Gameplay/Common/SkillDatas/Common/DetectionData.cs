using MessagePack;

namespace Goblin.Gameplay.Common.SkillDatas.Common
{
    /// <summary>
    /// 碰撞检测行为数据
    /// </summary>
    [MessagePackObject(true)]
    public abstract class DetectionData : SkillActionData
    {
        /// <summary>
        /// 平移
        /// </summary>
        public Vector3Data position { get; set; }
    }
}
