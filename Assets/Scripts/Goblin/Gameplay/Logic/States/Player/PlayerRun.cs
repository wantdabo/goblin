﻿using Goblin.Gameplay.Common.Defines;
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
        protected override List<uint> passes => new() { StateDef.PLAYER_IDLE };
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
            var motion = joystick.dire * 75 * FP.EN1 * tick;
            spatial.position += new TSVector(motion.x, 0, motion.y);
            if (0 < motion.x)
            {
                spatial.eulerAngle = TSVector.up * 90;
            }
            else if(0 > motion.x)
            {
                spatial.eulerAngle = TSVector.up * -90;
            }
        }
    }
}