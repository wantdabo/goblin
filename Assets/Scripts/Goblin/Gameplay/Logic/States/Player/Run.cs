using Goblin.Gameplay.Logic.Common.StateMachine;
using Goblin.Gameplay.Logic.Inputs;
using Goblin.Gameplay.Logic.Transform;
using System.Collections.Generic;
using TrueSync;

namespace Goblin.Gameplay.Logic.States.Player
{
    public class Run : State
    {
        public override uint id => PLAYER_RUN;
        protected override List<uint> passes => new() { PLAYER_IDLE };
        private Gamepad gamepad { get; set; }
        private Spatial spatial { get; set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            gamepad = machine.paramachine.actor.GetBehavior<Gamepad>();
            spatial = machine.paramachine.actor.GetBehavior<Spatial>();
        }
        public override bool OnCheck()
        {
            var joystick = gamepad.GetInput(InputType.Joystick);

            return joystick.press;
        }

        public override void OnTick(uint frame, FP fixedTick)
        {
            base.OnTick(frame, fixedTick);
            var joystick = gamepad.GetInput(InputType.Joystick);
            var motion = joystick.dire * 5 * fixedTick;
            spatial.position += new TSVector(motion.x, 0, motion.y);
        }
    }
}
