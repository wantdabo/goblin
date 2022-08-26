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
            ActorBehavior.Info.pos = new Vector3(0, 1, 0);
            ActorBehavior.Info.scale = new Vector4(Fix64.Half, 165 * Fix64.EN2, Fix64.Half, Fix64.One);

            AddBehavior<InputBehavior>();
            AddBehavior<ColliderBehavior<BoxCollider>>();
            AddBehavior<GravityBehavior>();
            AddBehavior<MotionBehavior>();
            AddBehavior<EnergyBehavior>();
            AddBehavior<HoshiBehavior>();
        }
    }
}
