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
    public class GravityBehavior : Behavior, IPLoop
    {
        private EnergyBehavior energyBehavior;
        protected override void OnCreate()
        {
            base.OnCreate();
            energyBehavior = Actor.GetBehavior<EnergyBehavior>();
        }

        public void PLoop(int frame, Fix64 detailTime)
        {
            var colliderBehavior = Actor.GetBehavior<ColliderBehavior>();
            if (null != colliderBehavior && colliderBehavior.IsOnGround) 
            {
                energyBehavior.Info.gravityVelocity = Vector3.Zero;
                return;
            }

            var motionBehavior = Actor.GetBehavior<MotionBehavior>();
            energyBehavior.Info.gravityVelocity += energyBehavior.Info.gravity * detailTime;
            motionBehavior?.AddForce(energyBehavior.Info.gravityVelocity * detailTime);
        }
    }
}
