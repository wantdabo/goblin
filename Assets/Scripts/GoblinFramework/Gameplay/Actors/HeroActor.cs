using GoblinFramework.Gameplay.Comps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Actors
{
    public class HeroActor : Actor
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            AddBehavior<InputComp, InputComp.InputInfo>();
            AddBehavior<MotionComp, MotionComp.MotionInfo>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}
