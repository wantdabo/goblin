using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.SkillDatas;
using Goblin.Gameplay.Logic.Skills.Action.Cache.Common;
using Goblin.Gameplay.Logic.Skills.Action.Common;
using TrueSync;

namespace Goblin.Gameplay.Logic.Skills.Action
{
    /// <summary>
    /// 技能印下 Buff 行为
    /// </summary>
    public class BuffStampEvent : SkillAction<BuffStampEventData, SkillActionCache>
    {
        public override ushort id => SKILL_ACTION_DEFINE.BUFF_STAMP_EVENT;
        

        protected override void OnExecute(BuffStampEventData data, SkillActionCache cache, uint frame, FP tick)
        {
        }
    }
}
