using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Logic.Common.StateMachine;
using Goblin.Gameplay.Logic.Inputs;
using System.Collections.Generic;

namespace Goblin.Gameplay.Logic.States.Player
{
    public class PlayerIdle : State
    {
        public override uint id => StateDef.PLAYER_IDLE;
        protected override List<uint> passes => new() { StateDef.PLAYER_RUN };
        private Gamepad gamepad { get; set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            gamepad = machine.paramachine.actor.GetBehavior<Gamepad>();
        }

        public override bool OnCheck()
        {
            var joystick = gamepad.GetInput(InputType.Joystick);
            
            return false == joystick.press;
        }
    }
}
