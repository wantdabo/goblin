using Goblin.Gameplay.Common.SkillData.Common;
using MessagePack;

namespace Goblin.Gameplay.Common.SkillData
{
    /// <summary>
    /// 音效行为数据
    /// </summary>
    [MessagePackObject(true)]
    public class SoundData : SkillActionData
    {
        /// <summary>
        /// 音效资源名
        /// </summary>
        public string res { get; set; }
    }
}
