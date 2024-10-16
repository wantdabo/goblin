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
            gamepad = sp.launcher.actor.GetBehavior<Gamepad>();
        }

        protected override void OnExecute(SkillBreakEventActionData data, FP tick)
        {
            if (BreakTokenType.NONE == data.token) return;

            if (BreakTokenType.JOYSTICK == (BreakTokenType.JOYSTICK & data.token) && null != gamepad)
            {
                var joystick = gamepad.GetInput(InputType.Joystick);
                if (joystick.press && joystick.dire != TSVector2.zero) sp.Break();
            }
            else if (BreakTokenType.RECV_HURT == (BreakTokenType.RECV_HURT & data.token))
            {
                // TODO 支持受击打断
            }
            else if (BreakTokenType.RECV_CONTROL == (BreakTokenType.RECV_CONTROL & data.token))
            {
                // TODO 支持控制打断
            }
            else if (BreakTokenType.SKILL_CAST == (BreakTokenType.SKILL_CAST & data.token))
            {
                // TODO 支持技能打断
            }
            else if (BreakTokenType.COMBO_SKILL_CAST == (BreakTokenType.COMBO_SKILL_CAST & data.token))
            {
                // TODO 支持连招技能打断
            }
        }
    }
}
