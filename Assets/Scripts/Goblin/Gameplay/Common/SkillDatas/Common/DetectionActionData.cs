using MessagePack;

namespace Goblin.Gameplay.Common.SkillDatas.Common
{
    /// <summary>
    /// 碰撞检测行为数据
    /// </summary>
    [MessagePackObject(true)]
    public abstract class DetectionActionData : SkillActionData
    {
        /// <summary>
        /// 检测次数
        /// </summary>
        public uint detectedcnt { get; set; }
        /// <summary>
        /// 平移
        /// </summary>
        public Vector3Data position { get; set; }
    }
}
