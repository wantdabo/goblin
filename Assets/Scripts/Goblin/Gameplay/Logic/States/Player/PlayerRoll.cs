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
    /// 玩家翻滚状态
    /// </summary>
    public class PlayerRoll : State
    {
        public override uint id => STATE_DEFINE.PLAYER_ROLL;

        protected override List<uint> passes => new() { STATE_DEFINE.PLAYER_FALLING };

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
            var bb = gamepad.GetInput(InputType.BB);

            return bb.press && physagent.grounded;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            elapsed = FP.Zero;
        }

        public override void OnExit()
        {
            base.OnExit();
            physagent.LossForce();
        }

        /// <summary>
        /// 持续时间/s
        /// </summary>
        private FP duration = 667 * FP.EN3;
        /// <summary>
        /// 时间流逝/s
        /// </summary>
        private FP elapsed = 0;
        public override void OnTick(uint frame, FP tick)
        {
            base.OnTick(frame, tick);
            elapsed += tick;
            if (elapsed >= duration)
            {
                Break();
                return;
            }

            var dire = physagent.rigidbody.rotation * FPVector3.forward;
            var motion = new FPVector3(dire.x, 0, dire.z) * attribute.movespeed;
            physagent.rigidbody.ApplyForce(motion);
        }
    }
}
