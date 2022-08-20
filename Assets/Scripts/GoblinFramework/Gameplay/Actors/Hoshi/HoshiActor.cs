using BEPUutilities;
using FixMath.NET;
using GoblinFramework.Gameplay.Actors.Hoshi.Behavior;
using GoblinFramework.Gameplay.Behaviors;
using GoblinFramework.Gameplay.Physics;
using GoblinFramework.Gameplay.Physics.Comps;
using GoblinFramework.General.Gameplay.RIL.RILS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Actors.Hoshi
{
    public class HoshiActor : Actor
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            AddBehavior<InputBehavior>();
            AddBehavior<MotionBehavior>();
            AddBehavior<GravityBehavior>();
            AddBehavior<HoshiBehavior>();
        }
    }
}
