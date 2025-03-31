using System;
using System.Collections.Generic;
using Goblin.Gameplay.Logic.Behaviors;

namespace Goblin.Gameplay.Logic.Common.Defines
{
    public class BEHAVIOR_TICKS
    {
        public static List<Type> TYPES { get; set; } = new()
        {
            typeof(Gamepad),
            typeof(StateMachine)
        };
    }
}