using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.SkillDatas;
using Goblin.Gameplay.Logic.Skills.Action.Cache.Common;
using Goblin.Gameplay.Logic.Skills.Action.Common;
using TrueSync;

namespace Goblin.Gameplay.Logic.Skills.Action
{
    /// <summary>
    /// 技能触发 Buff 行为
    /// </summary>
    public class BuffTriggerEvent : SkillAction<BuffTriggerEventData, SkillActionCache>
    {
        public override ushort id => SKILL_ACTION_DEFINE.BUFF_TRIGGER_EVENT;

        protected override void OnExecute(BuffTriggerEventData data, SkillActionCache cache, uint frame, FP tick)
        {
        }
    }
}
