using Goblin.Gameplay.Common;
using Goblin.Gameplay.Render.Common;
using Goblin.Gameplay.States.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrueSync;

namespace Goblin.Gameplay.States.Player
{
    public class PlayerRunningState : FSMState
    {
        protected override List<Type> passes => new() { typeof(PlayerIdleState) };

        public override bool OnCheck()
        {
            var joystick = fsm.sm.actor.GetBehavior<Gamepad>().GetInput(InputType.Joystick);

            return joystick.press;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            fsm.sm.actor.eventor.Tell(new PlayAnimEvent { animName = "Running", loop = true });
        }

        public override void OnFPTick(uint frame, FP fixedTick)
        {
            base.OnFPTick(frame, fixedTick);
            var joystick = fsm.sm.actor.GetBehavior<Gamepad>().GetInput(InputType.Joystick);
            fsm.sm.actor.GetBehavior<Node>().pos += joystick.dire * 10 * fixedTick;
            fsm.sm.actor.eventor.Tell(new SettingsMotionReverseEvent { motion = new UnityEngine.Vector2(joystick.dire.x.AsFloat(), joystick.dire.y.AsFloat()) });
        }
    }
}
