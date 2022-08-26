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
            var energyBehavior = Actor.GetBehavior<EnergyBehavior>();
            var colliderBehavior = Actor.GetBehavior<ColliderBehavior>();
            if (null != colliderBehavior && colliderBehavior.IsOnGround) return;
            energyBehavior.Info.linearEnergy += Info.gravity * detailTime;
        }

        #region GravityInfo
        public class GravityInfo : BehaviorInfo
        {
            public Vector3 gravity = new Vector3(0, -981 * Fix64.EN2, 0);
        }
        #endregion
    }
}
