using GoblinFramework.Gameplay.Behaviors;
using GoblinFramework.General.Gameplay.RIL.RILS;
using Numerics.Fixed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Actors.Hoshi.Behavior
{
    public class HoshiRun : HoshiState
    {
        protected override void OnEnter()
        {
            base.OnEnter();
            Actor.ActorBehavior.SendRIL<RILState>((ril) => ril.stateId = 2);

            var inputBehavior = Actor.GetBehavior<InputBehavior>();
            var motionBehavior = Actor.GetBehavior<MotionBehavior>();

            var joystick = inputBehavior.GetInput(InputType.Joystick);
            Fixed64Vector3 force = new Fixed64Vector3(joystick.dire.x, 0, joystick.dire.y);
            motionBehavior.AddForce(force * Behavior.Info.runSpeed);
        }
    }
}
