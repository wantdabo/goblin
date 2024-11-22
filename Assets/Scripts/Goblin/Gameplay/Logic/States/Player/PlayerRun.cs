using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Logic.Attributes;
using Goblin.Gameplay.Logic.Common.StateMachine;
using Goblin.Gameplay.Logic.Inputs;
using Goblin.Gameplay.Logic.Spatials;
using Kowtow.Math;
using System.Collections.Generic;

namespace Goblin.Gameplay.Logic.States.Player
{
    /// <summary>
    /// 玩家奔跑状态
    /// </summary>
    public class PlayerRun : State
    {
        public override uint id => STATE_DEFINE.PLAYER_RUN;
        
        protected override List<uint> passes => new() { STATE_DEFINE.PLAYER_IDLE, STATE_DEFINE.PLAYER_HURT, STATE_DEFINE.PLAYER_ATTACK };

        private Attribute attribute { get; set; }
        private Spatial spatial { get; set; }
        private Gamepad gamepad { get; set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            attribute = machine.paramachine.actor.GetBehavior<Attribute>();
            gamepad = machine.paramachine.actor.GetBehavior<Gamepad>();
            spatial = machine.paramachine.actor.GetBehavior<Spatial>();
        }
        public override bool OnValid()
        {
            var joystick = gamepad.GetInput(InputType.Joystick);

            return joystick.press;
        }

        public override void OnTick(uint frame, FP tick)
        {
            base.OnTick(frame, tick);
            var joystick = gamepad.GetInput(InputType.Joystick);
            var motion = joystick.dire * attribute.movespeed * tick;
            spatial.position += new FPVector3(motion.x, 0, motion.y);

            if (motion != FPVector2.zero)
            {
                FP angle = FPMath.Atan2(motion.x, motion.y) * FPMath.Rad2Deg;
                spatial.eulerAngles = FPVector3.up * angle;
            }
        }
    }
}
