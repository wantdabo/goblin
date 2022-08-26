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
    public class HoshiRun : HoshiState
    {
        public override bool OnDetectEnter()
        {
            return Behavior.InputBehavior.GetInput(InputType.Joystick).press;
        }

        public override bool OnDetectLeave()
        {
            return false == Behavior.InputBehavior.GetInput(InputType.Joystick).press;
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            Actor.ActorBehavior.SendRIL<RILState>((ril) => ril.stateName = "Run");
        }

        protected override void OnLeave()
        {
            base.OnLeave();
        }

        public override void OnStateTick(int frame, Fix64 detailTime)
        {
            base.OnStateTick(frame, detailTime);

            var joystick = Behavior.InputBehavior.GetInput(InputType.Joystick);

            if (joystick.press) Actor.ActorBehavior.Info.rotation = new Vector3(joystick.dire.X, 0, joystick.dire.Y);

            Vector3 force = new Vector3(joystick.dire.X, 0, joystick.dire.Y);
            Actor.GetBehavior<EnergyBehavior>().Info.momentEnergy += force * Behavior.Info.runSpeed * detailTime;
        }
    }
}
