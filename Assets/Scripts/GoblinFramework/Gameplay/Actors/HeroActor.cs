using GoblinFramework.Gameplay.Behaviors;
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
            AddBehavior<InputBehavior>();
            AddBehavior<MotionBehavior>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}
