using Goblin.Gameplay.Common.SkillData.Common;
using MessagePack;

namespace Goblin.Gameplay.Common.SkillData
{
    /// <summary>
    /// 动画行为数据
    /// </summary>
    [MessagePackObject(true)]
    public class AnimationData : SkillActionData
    {
        /// <summary>
        /// 动画名
        /// </summary>
        public string name { get; set; }
    }
}
