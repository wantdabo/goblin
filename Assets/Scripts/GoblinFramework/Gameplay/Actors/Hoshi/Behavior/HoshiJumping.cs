using BEPUutilities;
using FixMath.NET;
using GoblinFramework.Gameplay.Behaviors;
using GoblinFramework.General.Gameplay.RIL.RILS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Actors.Hoshi.Behavior
{
    public class HoshiJumping : HoshiState
    {
        public override List<Type> PassStates => null;

        public override bool OnDetectEnter()
        {
            var input = Behavior.InputBehavior.GetInput(InputType.BC);

            return Behavior.ColliderBehavior.IsOnGround && input.release;
        }

        public override bool OnDetectLeave()
        {
            return actor.GetBehavior<BouncingBehavior>().info.bouncingEnergy.Y <= Fix64.Zero;
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            actor.actorBehaivor.SendRIL<RILState>((ril) => ril.stateName = "Jumping");
            var force = Behavior.info.jumpMotionForce;
            var joystick = Behavior.InputBehavior.GetInput(InputType.Joystick);
            if (joystick.press) force += joystick.dire.ToVector3() * Behavior.info.runSpeed * (15 * Fix64.EN1);

            var bouncingBehavior = actor.GetBehavior<BouncingBehavior>();
            bouncingBehavior.info.bouncingEnergy += force;
        }
    }
}
