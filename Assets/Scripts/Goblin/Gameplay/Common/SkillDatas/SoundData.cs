using Goblin.Gameplay.Common.SkillDatas.Common;
using MessagePack;

namespace Goblin.Gameplay.Common.SkillDatas
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
