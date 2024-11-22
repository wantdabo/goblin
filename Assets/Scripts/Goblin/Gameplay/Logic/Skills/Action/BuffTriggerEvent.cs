using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.SkillDatas;
using Goblin.Gameplay.Logic.Skills.Action.Cache.Common;
using Goblin.Gameplay.Logic.Skills.Action.Common;

namespace Goblin.Gameplay.Logic.Skills.Action
{
    /// <summary>
    /// 技能触发 Buff 行为
    /// </summary>
    public class BuffTriggerEvent : SkillAction<BuffTriggerEventData, SkillActionCache>
    {
        public override ushort id => SKILL_ACTION_DEFINE.BUFF_TRIGGER_EVENT;

        protected override void OnEnter(BuffTriggerEventData data, SkillActionCache cache)
        {
            base.OnEnter(data, cache);
            pipeline.AddTriggerBuffData(data);
            if (BUFF_DEFINE.ACTIVE_SELF_TIMELINE != (data.triggerself & BUFF_DEFINE.ACTIVE_SELF_TIMELINE)) return;
            
            pipeline.launcher.actor.eventor.Tell(new Buffs.Common.BuffTriggerEvent()
            {
                id = data.buffid,
                from = pipeline.launcher.actor.id,
            });
        }

        protected override void OnExit(BuffTriggerEventData data, SkillActionCache cache)
        {
            base.OnExit(data, cache);
            pipeline.RmvTriggerBuffData(data);
        }
    }
}
