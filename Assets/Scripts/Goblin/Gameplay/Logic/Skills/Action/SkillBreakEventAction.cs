using Goblin.Gameplay.Common.Defines;
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

        protected override void OnExecute(SkillBreakEventActionData data, FP tick)
        {
            pipeline.SetBreakToken(data.token);

            if (BreakTokenDef.NONE == data.token) return;

            if (BreakTokenDef.JOYSTICK == (BreakTokenDef.JOYSTICK & data.token) && null != gamepad)
            {
                var joystick = gamepad.GetInput(InputType.Joystick);
                if (joystick.press && joystick.dire != TSVector2.zero) pipeline.Break();
            }
        }
    }
}
