using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.SkillDatas.Action;
using Goblin.Gameplay.Logic.Inputs;
using Goblin.Gameplay.Logic.Skills.Action.Common;
using TrueSync;

namespace Goblin.Gameplay.Logic.Skills.Action
{
    /// <summary>
    /// 技能打断标记行为
    /// </summary>
    public class BreakEventAction : SkillAction<BreakEventActionData>
    {
        public override ushort id => SkillActionDef.BREAK_EVENT;

        private Gamepad gamepad { get; set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            gamepad = pipeline.launcher.actor.GetBehavior<Gamepad>();
        }

        protected override void OnEnter(BreakEventActionData data)
        {
            pipeline.StampBreakToken(data.token);
        }

        protected override void OnExecute(BreakEventActionData data, uint frame, FP tick)
        {
            if (BreakTokenDef.NONE == data.token) return;

            if (BreakTokenDef.JOYSTICK == (BreakTokenDef.JOYSTICK & pipeline.breaktoken) && null != gamepad)
            {
                var joystick = gamepad.GetInput(InputType.Joystick);
                if (joystick.press && joystick.dire != TSVector2.zero) pipeline.Break();
            }
        }

        protected override void OnExit(BreakEventActionData data)
        {
            pipeline.EraseBreakToken(data.token);
        }
    }
}
