using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Logic.Attributes;
using Goblin.Gameplay.Logic.Common.StateMachine;
using Goblin.Gameplay.Logic.Inputs;
using Goblin.Gameplay.Logic.Physics;
using Goblin.Gameplay.Logic.Spatials;
using Kowtow.Math;
using System.Collections.Generic;

namespace Goblin.Gameplay.Logic.States.Player
{
    /// <summary>
    /// 玩家下落状态
    /// </summary>
    public class PlayerFalling : State
    {
        public override uint id => STATE_DEFINE.PLAYER_FALLING;

        protected override List<uint> passes => new() { STATE_DEFINE.PLAYER_FALLING2GROUND, STATE_DEFINE.PLAYER_ATTACK };

        private Attribute attribute { get; set; }
        private Spatial spatial { get; set; }
        private PhysAgent physagent { get; set; }
        private Gamepad gamepad { get; set; }


        protected override void OnCreate()
        {
            base.OnCreate();
            attribute = machine.paramachine.actor.GetBehavior<Attribute>();
            gamepad = machine.paramachine.actor.GetBehavior<Gamepad>();
            spatial = machine.paramachine.actor.GetBehavior<Spatial>();
            physagent = machine.paramachine.actor.GetBehavior<PhysAgent>();
        }

        public override bool OnValid()
        {
            return false == physagent.grounded && physagent.rigidbody.velocity.y < 0;
        }

        public override void OnTick(uint frame, FP tick)
        {
            base.OnTick(frame, tick);
            var joystick = gamepad.GetInput(InputType.Joystick);
            var motion = joystick.dire * attribute.movespeed * FP.Half * FP.Half;
            physagent.rigidbody.ApplyForce(new FPVector3(motion.x, 0, FP.Zero));

            if (motion != FPVector2.zero)
            {
                FP angle = FPMath.Atan2(motion.x, motion.y) * FPMath.Rad2Deg;
                spatial.eulerAngles = FPVector3.up * angle;
            }
        }
    }
}
