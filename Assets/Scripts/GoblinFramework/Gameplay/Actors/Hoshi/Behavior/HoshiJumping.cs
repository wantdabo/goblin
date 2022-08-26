using BEPUutilities;
using FixMath.NET;
using GoblinFramework.Gameplay.Behaviors;
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
            var input = Behavior.InputBehavior.GetInput(Behaviors.InputType.BC);

            return Behavior.ColliderBehavior.IsOnGround && input.release;
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            var bouncingBehavior = Actor.GetBehavior<BouncingBehavior>();
            bouncingBehavior.Info.bouncingVelocity = Behavior.Info.jumpMotionForce + Behavior.MotionBehavior.Info.motionForce;

            var joystick = Behavior.InputBehavior.GetInput(Behaviors.InputType.Joystick);
            if (joystick.press)
            {
                var motionForce = joystick.dire * Behavior.Info.runSpeed;
                bouncingBehavior.Info.bouncingVelocity.X += motionForce.X;
                bouncingBehavior.Info.bouncingVelocity.Z += motionForce.Y;
            }
        }

        protected override void OnLeave()
        {
            base.OnLeave();
        }
    }
}
