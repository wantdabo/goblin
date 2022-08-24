using BEPUutilities;
using FixMath.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Behaviors
{
    public class EnergyBehavior : Behavior<EnergyBehavior.EnergyInfo>
    {

        public class EnergyInfo : BehaviorInfo 
        {
            public Vector3 gravity = new Vector3(0, -981 * Fix64.EN2, 0);
            public Vector3 gravityVelocity = Vector3.Zero;
        }
    }
}
