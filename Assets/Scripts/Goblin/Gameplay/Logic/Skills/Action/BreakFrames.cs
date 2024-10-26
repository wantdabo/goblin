using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.SkillDatas.Action;
using Goblin.Gameplay.Common.SkillDatas.Action.Common;
using Goblin.Gameplay.Logic.Skills.Action.Common;
using Goblin.Gameplay.Logic.Skills.ActionCache.Common;
using TrueSync;

namespace Goblin.Gameplay.Logic.Skills.Action
{
    /// <summary>
    /// 技能跳帧行为
    /// </summary>
    public class BreakFrames : SkillAction<BreakFramesData, SkillActionCache>
    {
        public override ushort id => SkillActionDef.BREAK_FRAMES_EVENT;

        protected override void OnEnter(BreakFramesData data, SkillActionCache cache)
        {
            base.OnEnter(data, cache);
            pipeline.seflbreakframes += data.selfbreakframes;
            pipeline.targetbreakframes = data.targetbreakframes;
        }

        protected override void OnExecute(BreakFramesData data, SkillActionCache cache, uint frame, FP tick)
        {
        }

        protected override void OnExit(BreakFramesData data, SkillActionCache cache)
        {
            base.OnExit(data, cache);
            pipeline.seflbreakframes -= data.selfbreakframes;
            pipeline.targetbreakframes -= data.targetbreakframes;
        }
    }
}
