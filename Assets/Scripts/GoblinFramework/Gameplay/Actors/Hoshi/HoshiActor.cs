using BEPUutilities;
using FixMath.NET;
using GoblinFramework.Gameplay.Actors.Hoshi.Behavior;
using GoblinFramework.Gameplay.Behaviors;
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

            ActorBehavior.Info.size = new Vector3(Fix64.Half, 165 * Fix64.EN2, Fix64.Half);

            AddComp<ActorCollider<BoxCollider>>();

            AddBehavior<InputBehavior>();
            AddBehavior<MotionBehavior>();
            AddBehavior<HoshiBehavior>();
        }
    }
}
