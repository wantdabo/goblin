using GoblinFramework.Gameplay.Common;
using GoblinFramework.General.Gameplay.RIL.RILS;
using Numerics.Fixed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Behaviors
{
    public class MotionBehavior : Behavior<MotionBehavior.MotionInfo>, IPLoop
    {
        public void AddForce(Fixed64Vector3 force)
        {
            Info.force += force;
        }

        public void PLoop(int frame)
        {
            if (Fixed64Vector3.zero == Info.force) return;

            Actor.ActorBehavior.Info.pos = Actor.ActorBehavior.Info.pos + Info.force;
            Actor.ActorBehavior.SendRIL<RILPos>((ril) =>
            {
                ril.x = Actor.ActorBehavior.Info.pos.x.AsFloat();
                ril.y = Actor.ActorBehavior.Info.pos.y.AsFloat();
                ril.z = Actor.ActorBehavior.Info.pos.z.AsFloat();
                ril.dire = 0;
            });
            Info.force = Fixed64Vector3.zero;
        }

        #region MotionInfo
        public class MotionInfo: BehaviorInfo
        {
            public Fixed64Vector3 force;
        }
        #endregion
    }
}
