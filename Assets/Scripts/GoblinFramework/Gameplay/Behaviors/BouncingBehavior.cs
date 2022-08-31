using BEPUutilities;
using FixMath.NET;
using GoblinFramework.Gameplay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Behaviors
{
    public class BouncingBehavior : Behavior<BouncingBehavior.BouncingInfo>, IPLoop
    {
        public void PLoop(int frame, Fix64 detailTime)
        {
            var gravityBehavior = actor.GetBehavior<GravityBehavior>();
            if (null == gravityBehavior) return;
            var gravityMag = gravityBehavior.info.gravityEnergy.Length();

            if (Fix64.Zero == info.bouncingEnergy.Y && Fix64.Zero == gravityMag) info.bouncingEnergy = Vector3.Zero;
            if (info.bouncingEnergy == Vector3.Zero) return;

            info.bouncingEnergy.Y -= gravityMag * Fix64.Half;
            info.bouncingEnergy.Y = Fix64Math.Clamp(info.bouncingEnergy.Y, 0, Fix64.MaxValue);
        }

        public class BouncingInfo : BehaviorInfo
        {
            public Vector3 bouncingEnergy;
        }
    }
}
