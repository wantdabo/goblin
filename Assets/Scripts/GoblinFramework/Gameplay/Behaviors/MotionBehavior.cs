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
    public class MotionBehavior : Behavior<MotionBehavior.MotionInfo>, IPLoop
    {
        public void AddForce(Vector3 force)
        {
            Info.motionForce += force;
        }

        public void PLoop(int frame, Fix64 detailTime)
        {
            if (Vector3.zero == Info.motionForce) return;
            var lossForce = Info.motionForce;
            Actor.ActorBehavior.Info.pos = Actor.ActorBehavior.Info.pos + lossForce;
            Actor.ActorBehavior.SendRIL<RILPos>((ril) =>
            {
                ril.x = Actor.ActorBehavior.Info.pos.X.AsFloat();
                ril.y = Actor.ActorBehavior.Info.pos.Y.AsFloat();
                ril.z = Actor.ActorBehavior.Info.pos.Z.AsFloat();
                ril.dire = 0;
            });
            Info.motionForce -= lossForce;
        }

        #region MotionInfo
        public class MotionInfo: BehaviorInfo
        {
            public Vector3 motionForce;
        }
        #endregion
    }
}
