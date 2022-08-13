using BEPUutilities;
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
            Info.force += force;
        }

        public void PLoop(int frame)
        {
            if (Vector3.zero == Info.force) return;

            Actor.ActorBehavior.Info.pos = Actor.ActorBehavior.Info.pos + Info.force;
            Actor.ActorBehavior.SendRIL<RILPos>((ril) =>
            {
                ril.x = Actor.ActorBehavior.Info.pos.X.AsFloat();
                ril.y = Actor.ActorBehavior.Info.pos.Y.AsFloat();
                ril.z = Actor.ActorBehavior.Info.pos.Z.AsFloat();
                ril.dire = 0;
            });
            Info.force = Vector3.zero;
        }

        #region MotionInfo
        public class MotionInfo: BehaviorInfo
        {
            public Vector3 force;
        }
        #endregion
    }
}
