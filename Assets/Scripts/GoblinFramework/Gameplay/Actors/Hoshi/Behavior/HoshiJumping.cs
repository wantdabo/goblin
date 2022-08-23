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
            Behavior.Info.jumpingForce = Behavior.Info.jumpMotionForce + Behavior.MotionBehavior.Info.motionForce;

            var joystick = Behavior.InputBehavior.GetInput(Behaviors.InputType.Joystick);
            if (joystick.press)
            {
                var motionForce = joystick.dire * Behavior.Info.runSpeed;
                Behavior.Info.jumpingForce.X += motionForce.X;
                Behavior.Info.jumpingForce.Z += motionForce.Y;
            }
        }

        protected override void OnLeave()
        {
            base.OnLeave();
        }

        public override void OnStateTick(int frame, Fix64 detailTime)
        {
            base.OnStateTick(frame, detailTime);
            var lossForce = Behavior.Info.jumpMotionForce * detailTime;
            Behavior.Info.jumpingForce -= lossForce;
            Behavior.MotionBehavior.AddForce(lossForce);
        }
    }
}
