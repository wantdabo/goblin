using MessagePack;

namespace Goblin.Gameplay.Common.SkillDatas.Action.Common
{
    /// <summary>
    /// 碰撞检测行为数据
    /// </summary>
    [MessagePackObject(true)]
    public abstract class DetectionActionData : SkillActionData
    {
        /// <summary>
        /// 碰撞检测平移
        /// </summary>
        public Vector3Data position { get; set; }        
    }
}
