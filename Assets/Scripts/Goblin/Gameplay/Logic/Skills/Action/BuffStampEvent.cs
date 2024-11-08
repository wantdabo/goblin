using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.SkillDatas;
using Goblin.Gameplay.Common.SkillDatas.Common;
using Goblin.Gameplay.Logic.Skills.Action.Cache;
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

        protected override void OnEnter(BuffStampEventData data, SkillActionCache cache)
        {
            base.OnEnter(data, cache);
            pipeline.AddStampBuffData(data);
            
            if (BUFF_DEFINE.ACTIVE_SELF_TIMELINE != (data.stampself & BUFF_DEFINE.ACTIVE_SELF_TIMELINE)) return;
            pipeline.launcher.actor.eventor.Tell(new Buffs.Common.BuffStampEvent()
            {
                id = data.buffid,
                layer = data.layer,
                from = pipeline.launcher.actor.id,
            });
        }

        protected override void OnExit(BuffStampEventData data, SkillActionCache cache)
        {
            base.OnExit(data, cache);
            pipeline.RmvStampBuffData(data);
        }
    }
}
