using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.SkillDatas;
using Goblin.Gameplay.Logic.Inputs;
using Goblin.Gameplay.Logic.Skills.Action.Cache.Common;
using Goblin.Gameplay.Logic.Skills.Action.Common;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Skills.Action
{
    /// <summary>
    /// 技能打断标记行为
    /// </summary>
    public class BreakTokenEvent : SkillAction<BreakTokenEventData, SkillActionCache>
    {
        public override ushort id => SKILL_ACTION_DEFINE.BREAK_TOKEN_EVENT;

        private Gamepad gamepad { get; set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            gamepad = pipeline.launcher.actor.GetBehavior<Gamepad>();
        }

        protected override void OnEnter(BreakTokenEventData data, SkillActionCache cache)
        {
            base.OnEnter(data, cache);
            pipeline.StampBreakToken(data.token);
        }
        
        protected override void OnExit(BreakTokenEventData data, SkillActionCache cache)
        {
            base.OnExit(data, cache);
            pipeline.EraseBreakToken(data.token);
        }

        protected override void OnExecute(BreakTokenEventData data, SkillActionCache cache, uint frame, FP tick)
        {
            if (BREAK_TOKEN_DEFINE.NONE == data.token) return;

            if (BREAK_TOKEN_DEFINE.JOYSTICK == (BREAK_TOKEN_DEFINE.JOYSTICK & pipeline.breaktoken) && null != gamepad)
            {
                var joystick = gamepad.GetInput(InputType.Joystick);
                if (joystick.press && joystick.dire != FPVector2.zero) pipeline.Break();
            }
        }
    }
}
