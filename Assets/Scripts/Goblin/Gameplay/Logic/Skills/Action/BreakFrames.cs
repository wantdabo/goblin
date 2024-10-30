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
    public class BreakFrames : SkillAction<BreakFramesData, SkillActionCache>
    {
        public override ushort id => SKILL_ACTION_DEFINE.BREAK_FRAMES_EVENT;

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
