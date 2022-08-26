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
            if (Info.bouncingVelocity == Vector3.Zero) return;
            Info.bouncingVelocity -= Info.bouncingVelocity * detailTime;
            if (Info.bouncingVelocity.Length() <= F64.C0p1) Info.bouncingVelocity = Vector3.Zero;
            General.GoblinDebug.Log(Info.bouncingVelocity.ToString());
        }

        #region BouncingInfo
        public class BouncingInfo : BehaviorInfo
        {
            public Vector3 bouncingVelocity = Vector3.Zero;
        }
        #endregion
    }
}
