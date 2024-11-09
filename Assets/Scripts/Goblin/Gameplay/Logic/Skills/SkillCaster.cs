using Goblin.Core;
using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Inputs;
using Goblin.Gameplay.Logic.Spatials;
using System.Collections.Generic;
using TrueSync;

namespace Goblin.Gameplay.Logic.Skills
{
    /// <summary>
    /// 技能释放解析
    /// </summary>
    public class SkillCaster : Comp
    {
        public SkillLauncher launcher { get; set; }
        private Spatial spatial { get; set; }
        private Gamepad gamepad { get; set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            spatial = launcher.actor.GetBehavior<Spatial>();
            gamepad = launcher.actor.GetBehavior<Gamepad>();
        }

        private void JoystickForawrd()
        {
            var joystick = gamepad.GetInput(InputType.Joystick);
            if (joystick.press)
            {
                if (TSVector2.zero != joystick.dire)
                {
                    FP angle = TSMath.Atan2(joystick.dire.x, joystick.dire.y) * TSMath.Rad2Deg;
                    spatial.eulerAngles = TSVector.up * angle;
                }
            }
        }

        public void OnExecute(FP tick)
        {
        }
    }
}
