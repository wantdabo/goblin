using GoblinFramework.Gameplay.Actors.Hoshi.Behavior;
using GoblinFramework.Gameplay.Behaviors;
using GoblinFramework.General.Gameplay.Command.Cmds;
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
            AddBehavior<HoshiBehavior>();
        }
    }
}
