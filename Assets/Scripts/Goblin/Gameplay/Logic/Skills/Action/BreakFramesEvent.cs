using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.SkillDatas;
using Goblin.Gameplay.Logic.Skills.Action.Cache.Common;
using Goblin.Gameplay.Logic.Skills.Action.Common;
using TrueSync;

namespace Goblin.Gameplay.Logic.Skills.Action
{
    /// <summary>
    /// 技能跳帧行为
    /// </summary>
    public class BreakFramesEvent : SkillAction<BreakFramesEventData, SkillActionCache>
    {
        public override ushort id => SKILL_ACTION_DEFINE.BREAK_FRAMES_EVENT;

        protected override void OnEnter(BreakFramesEventData eventData, SkillActionCache cache)
        {
            base.OnEnter(eventData, cache);
            pipeline.seflbreakframes += eventData.selfbreakframes;
            pipeline.targetbreakframes = eventData.targetbreakframes;
        }
        
        protected override void OnExit(BreakFramesEventData eventData, SkillActionCache cache)
        {
            base.OnExit(eventData, cache);
            pipeline.seflbreakframes -= eventData.selfbreakframes;
            pipeline.targetbreakframes -= eventData.targetbreakframes;
        }
    }
}
