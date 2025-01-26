using Goblin.Core;
using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.StateMachine;
using Goblin.Gameplay.Logic.Inputs;
using Goblin.Gameplay.Logic.Physics;
using Goblin.Gameplay.Logic.Spatials;
using Kowtow.Math;
using System.Collections.Generic;

namespace Goblin.Gameplay.Logic.Skills
{
    /// <summary>
    /// 技能释放解析
    /// </summary>
    public class SkillCaster : Comp
    {
        public SkillLauncher launcher { get; set; }
        private PhysAgent physagent { get; set; }
        private ParallelMachine paramachine { get; set; }
        private Spatial spatial { get; set; }
        private Gamepad gamepad { get; set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            physagent = launcher.actor.GetBehavior<PhysAgent>();
            paramachine = launcher.actor.GetBehavior<ParallelMachine>();
            spatial = launcher.actor.GetBehavior<Spatial>();
            gamepad = launcher.actor.GetBehavior<Gamepad>();
        }

        private void JoystickForawrd()
        {
            var joystick = gamepad.GetInput(InputType.Joystick);
            if (joystick.press)
            {
                if (FPVector2.zero != joystick.dire)
                {
                    FP angle = FPMath.Atan2(joystick.dire.x, joystick.dire.y) * FPMath.Rad2Deg;
                    spatial.eulerAngles = FPVector3.up * angle;
                }
            }
        }

        public void OnExecute(FP tick)
        {
            var machine = paramachine.GetMachine();
            if (null == machine || null == machine.current || (null != machine.current.aisles && false == machine.current.aisles.Contains(STATE_DEFINE.PLAYER_ATTACK))) return;
            
            InputInfo input = default;
            if (false == physagent.grounded)
            {
                JoystickForawrd();
                input = gamepad.GetInput(InputType.BC);
                if (input.release)
                {
                    launcher.Load(10011);
                    launcher.Launch(10011);
                }
                
                return;
            }
            
            input = gamepad.GetInput(InputType.BC);
            if (input.release)
            {
                JoystickForawrd();
                launcher.Load(10001);
                launcher.Launch(10001);
            }
        }
    }
}
