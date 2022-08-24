using BEPUutilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Behaviors
{
    public class BouncingBehavior : Behavior<BouncingBehavior.BouncingInfo>
    {
        #region BouncingInfo
        public class BouncingInfo : BehaviorInfo
        {
            public Vector3 bouncingVelocity = Vector3.Zero;
        }
        #endregion
    }
}
