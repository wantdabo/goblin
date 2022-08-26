using BEPUutilities;
using FixMath.NET;
using GoblinFramework.Gameplay.Common;
using GoblinFramework.General.Gameplay.RIL.RILS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Behaviors
{
    public class MotionBehavior : Behavior
    {
        public void Motion(Vector3 motionForce)
        {
            if (Vector3.Zero == motionForce) return;

            Actor.ActorBehavior.Info.pos = Actor.ActorBehavior.Info.pos + motionForce;
            Actor.ActorBehavior.SendRIL<RILPos>((ril) =>
            {
                ril.x = Actor.ActorBehavior.Info.pos.X.AsFloat();
                ril.y = Actor.ActorBehavior.Info.pos.Y.AsFloat();
                ril.z = Actor.ActorBehavior.Info.pos.Z.AsFloat();
                ril.dire = 0;
            });
        }
    }
}
