using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Logic.Common.StateMachine;
using Goblin.Gameplay.Logic.Inputs;
using Goblin.Gameplay.Logic.Spatials;
using System.Collections.Generic;
using TrueSync;

namespace Goblin.Gameplay.Logic.States.Player
{
    public class PlayerRun : State
    {
        public override uint id => StateDef.PLAYER_RUN;
        protected override List<uint> passes => new() { StateDef.PLAYER_IDLE, StateDef.PLAYER_ATTACK };
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

        public override void OnTick(uint frame, FP tick)
        {
            base.OnTick(frame, tick);
            var joystick = gamepad.GetInput(InputType.Joystick);
            var motion = joystick.dire * 55 * FP.EN1 * tick;
            spatial.position += new TSVector(motion.x, 0, motion.y);

            if (motion != TSVector2.zero)
            {
                FP angle = TSMath.Atan2(motion.x, motion.y) * TSMath.Rad2Deg;
                spatial.eulerAngle = TSVector.up * angle;
            }
        }
    }
}
