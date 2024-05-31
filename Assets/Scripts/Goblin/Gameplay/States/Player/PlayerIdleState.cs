using Goblin.Gameplay.Common;
using Goblin.Gameplay.Render.Common;
using Goblin.Gameplay.States.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goblin.Gameplay.States.Player
{
    public class PlayerIdleState : FSMState
    {
        protected override List<Type> passes => new() { typeof(PlayerRunningState) };

        public override bool OnCheck()
        {
            var joystick = fsm.sm.actor.GetBehavior<Gamepad>().GetInput( InputType.Joystick);

            return false == joystick.press;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            fsm.sm.actor.eventor.Tell(new PlayAnimEvent { animName = "Idle", loop = true });
        }
    }
}
