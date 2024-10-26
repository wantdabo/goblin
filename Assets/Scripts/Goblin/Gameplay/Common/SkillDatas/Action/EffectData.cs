using Goblin.Gameplay.Common.SkillDatas.Action.Common;
using MessagePack;

namespace Goblin.Gameplay.Common.SkillDatas.Action
{
    /// <summary>
    /// 特效行为数据
    /// </summary>
    [MessagePackObject(true)]
    public class EffectData : SkillActionData
    {
        /// <summary>
        /// 特效资源名
        /// </summary>
        public string res { get; set; }
        /// <summary>
        /// 特效平移
        /// </summary>
        public Vector3Data position { get; set; }
        /// <summary>
        /// 特效欧拉旋转
        /// </summary>
        public Vector3Data eulerAngle { get; set; }
        /// <summary>
        /// 特效缩放
        /// </summary>
        public int scale { get; set; }
        /// <summary>
        /// 特效绑定
        /// </summary>
        public bool binding { get; set; }
    }
}
