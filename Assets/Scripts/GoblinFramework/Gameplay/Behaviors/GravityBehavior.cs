using BEPUutilities;
using FixMath.NET;
using GoblinFramework.Gameplay.Common;
using GoblinFramework.Gameplay.Physics.Comps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Behaviors
{
    public class GravityBehavior : Behavior<GravityBehavior.GravityInfo>, IPLoop
    {
        public void PLoop(int frame, Fix64 detailTime)
        {
            var colliderBehavior = actor.GetBehavior<ColliderBehavior>();
            if (null != colliderBehavior && colliderBehavior.IsOnGround) { info.gravityEnergy = Vector3.Zero; return; }
            info.gravityEnergy += info.gravity * detailTime;
        }

        #region GravityInfo
        public class GravityInfo : BehaviorInfo
        {
            public Vector3 gravity = new Vector3(0, -981 * Fix64.EN2, 0);
            public Vector3 gravityEnergy = Vector3.Zero;
        }
        #endregion
    }
}
