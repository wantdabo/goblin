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
            var gravityBehavior = Actor.GetBehavior<GravityBehavior>();
            if (null == gravityBehavior) return;
            var gravityMag = gravityBehavior.Info.gravityEnergy.Length();

            if (Fix64.Zero == Info.bouncingEnergy.Y && Fix64.Zero == gravityMag) Info.bouncingEnergy = Vector3.Zero;
            if (Info.bouncingEnergy == Vector3.Zero) return;

            Info.bouncingEnergy.Y -= gravityMag * Fix64.Half;
            Info.bouncingEnergy.Y = Fix64Math.Clamp(Info.bouncingEnergy.Y, 0, Fix64.MaxValue);
        }

        public class BouncingInfo : BehaviorInfo
        {
            public Vector3 bouncingEnergy;
        }
    }
}
