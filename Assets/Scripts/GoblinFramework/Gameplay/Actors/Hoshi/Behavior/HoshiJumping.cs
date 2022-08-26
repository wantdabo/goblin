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
            var input = Behavior.InputBehavior.GetInput(InputType.BC);

            return Behavior.ColliderBehavior.IsOnGround && input.release;
        }

        public override bool OnDetectLeave()
        {
            return Actor.GetBehavior<EnergyBehavior>().Info.linearEnergy.Y <= Fix64.Zero;
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            General.GoblinDebug.Log("========> Jumping Enter");

            var force = Behavior.Info.jumpMotionForce;
            var joystick = Behavior.InputBehavior.GetInput(InputType.Joystick);
            if (joystick.press) force += joystick.dire.ToVector3() * Behavior.Info.runSpeed * (15 * Fix64.EN1);

            var energyBehavior = Actor.GetBehavior<EnergyBehavior>();
            energyBehavior.Info.linearEnergy += force;
        }

        protected override void OnLeave()
        {
            base.OnLeave();
            General.GoblinDebug.Log("========> Jumping Leave");
        }

        public override void OnStateTick(int frame, Fix64 detailTime)
        {
            base.OnStateTick(frame, detailTime);
            General.GoblinDebug.Log("========> Jumping");
        }
    }
}
