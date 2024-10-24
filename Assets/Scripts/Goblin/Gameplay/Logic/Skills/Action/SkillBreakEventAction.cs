﻿using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.SkillDatas.Action;
using Goblin.Gameplay.Logic.Inputs;
using Goblin.Gameplay.Logic.Skills.Action.Common;
using TrueSync;

namespace Goblin.Gameplay.Logic.Skills.Action
{
    public class SkillBreakEventAction : SkillAction<SkillBreakEventActionData>
    {
        public override ushort id => SkillActionDef.SKILL_BREAK_EVENT;

        private Gamepad gamepad { get; set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            gamepad = pipeline.launcher.actor.GetBehavior<Gamepad>();
        }

        protected override void OnEnter(SkillBreakEventActionData data)
        {
            pipeline.StampBreakToken(data.token);
        }

        protected override void OnExecute(SkillBreakEventActionData data, uint frame, FP tick)
        {
            if (BreakTokenDef.NONE == data.token) return;

            if (BreakTokenDef.JOYSTICK == (BreakTokenDef.JOYSTICK & pipeline.breaktoken) && null != gamepad)
            {
                var joystick = gamepad.GetInput(InputType.Joystick);
                if (joystick.press && joystick.dire != TSVector2.zero) pipeline.Break();
            }
        }

        protected override void OnExit(SkillBreakEventActionData data)
        {
            pipeline.EraseBreakToken(data.token);
        }
    }
}
