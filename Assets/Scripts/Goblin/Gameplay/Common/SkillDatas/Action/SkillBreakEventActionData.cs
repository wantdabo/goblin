using Goblin.Gameplay.Common.SkillDatas.Action.Common;
using MessagePack;

namespace Goblin.Gameplay.Common.SkillDatas.Action
{
    [MessagePackObject(true)]
    public class SkillBreakEventActionData : SkillActionData
    {
        public int token { get; set; }
    }
}
