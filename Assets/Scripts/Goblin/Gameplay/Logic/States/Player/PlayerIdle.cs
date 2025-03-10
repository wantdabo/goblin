﻿using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Logic.Common.StateMachine;
using Goblin.Gameplay.Logic.Inputs;
using Goblin.Gameplay.Logic.Physics;
using System.Collections.Generic;

namespace Goblin.Gameplay.Logic.States.Player
{
    /// <summary>
    /// 玩家空闲状态
    /// </summary>
    public class PlayerIdle : State
    {
        public override uint id => STATE_DEFINE.PLAYER_IDLE;

        protected override List<uint> passes => new() { STATE_DEFINE.PLAYER_RUN, STATE_DEFINE.PLAYER_FALLING, STATE_DEFINE.PLAYER_JUMP_START, STATE_DEFINE.PLAYER_ROLL, STATE_DEFINE.PLAYER_HURT, STATE_DEFINE.PLAYER_ATTACK };

        private Gamepad gamepad { get; set; }
        private PhysAgent physagent { get; set; }


        protected override void OnCreate()
        {
            base.OnCreate();
            gamepad = machine.paramachine.actor.GetBehavior<Gamepad>();
            physagent = machine.paramachine.actor.GetBehavior<PhysAgent>();
        }

        public override bool OnValid()
        {
            var joystick = gamepad.GetInput(InputType.Joystick);

            return false == joystick.press && physagent.grounded;
        }
    }
}
