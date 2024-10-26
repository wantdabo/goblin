using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.SkillDatas.Action;
using Goblin.Gameplay.Common.SkillDatas.Action.Common;
using Goblin.Gameplay.Logic.Skills.Action.Common;
using TrueSync;

namespace Goblin.Gameplay.Logic.Skills.Action
{
    /// <summary>
    /// 技能跳帧行为
    /// </summary>
    public class SkillBreakFramesAction : SkillAction<SkillBreakFramesActionData>
    {
        public override ushort id => SkillActionDef.SKILL_BREAK_FRAMES_EVENT;

        protected override void OnEnter(SkillBreakFramesActionData data)
        {
            base.OnEnter(data);
            pipeline.seflbreakframes += data.selfbreakframes;
            pipeline.targetbreakframes = data.targetbreakframes;
        }

        protected override void OnExecute(SkillBreakFramesActionData data, uint frame, FP tick)
        {
        }

        protected override void OnExit(SkillBreakFramesActionData data)
        {
            base.OnExit(data);
            pipeline.seflbreakframes -= data.selfbreakframes;
            pipeline.targetbreakframes -= data.targetbreakframes;
        }
    }
}
