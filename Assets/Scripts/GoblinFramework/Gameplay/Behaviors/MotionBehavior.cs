using BEPUutilities;
using FixMath.NET;
using GoblinFramework.Gameplay.Common;
using GoblinFramework.Common.Gameplay.RIL.RILS;
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

            actor.actorBehaivor.info.pos = actor.actorBehaivor.info.pos + motionForce;
            actor.actorBehaivor.SendRIL<RILPos>((ril) =>
            {
                ril.x = actor.actorBehaivor.info.pos.X.AsFloat();
                ril.y = actor.actorBehaivor.info.pos.Y.AsFloat();
                ril.z = actor.actorBehaivor.info.pos.Z.AsFloat();
                ril.dire = 0;
            });
        }
    }
}
