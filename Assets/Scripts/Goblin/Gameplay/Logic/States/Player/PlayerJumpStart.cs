using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Logic.Attributes;
using Goblin.Gameplay.Logic.Common.StateMachine;
using Goblin.Gameplay.Logic.Inputs;
using Goblin.Gameplay.Logic.Physics;
using Kowtow.Math;
using System.Collections.Generic;

namespace Goblin.Gameplay.Logic.States.Player
{
    /// <summary>
    /// 玩家跳跃开始状态
    /// </summary>
    public class PlayerJumpStart : State
    {
        public override uint id => STATE_DEFINE.PLAYER_JUMP_START;

        protected override List<uint> passes => new() { STATE_DEFINE.PLAYER_JUMPING, STATE_DEFINE.PLAYER_FALLING2GROUND };

        private Attribute attribute { get; set; }
        private Gamepad gamepad { get; set; }
        private PhysAgent physagent { get; set; }


        protected override void OnCreate()
        {
            base.OnCreate();
            attribute = machine.paramachine.actor.GetBehavior<Attribute>();
            gamepad = machine.paramachine.actor.GetBehavior<Gamepad>();
            physagent = machine.paramachine.actor.GetBehavior<PhysAgent>();
        }

        public override bool OnValid()
        {
            var ba = gamepad.GetInput(InputType.BA);

            return ba.press && physagent.grounded;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            var joystick = gamepad.GetInput(InputType.Joystick);
            var motion = FPVector3.up + (joystick.press ? new FPVector3(joystick.dire.x, 0, FP.Zero) * FP.Half : FPVector3.zero);
            physagent.rigidbody.ApplyImpulse(motion * attribute.jumpforce);
        }
    }
}
