using System;
using System.Collections.Generic;
using Goblin.Gameplay.Behaviors;
using Goblin.Gameplay.Logic.Behaviors;

namespace Goblin.Gameplay.Logic.Common.Defines
{
    /// <summary>
    /// Tick 定义
    /// </summary>
    public class TICK_DEFINE
    {
        /// <summary>
        /// Tick 时序
        /// </summary>
        public static List<Type> TICK_TYPE_LIST { get; private set; } = new()
        {
            typeof(Gamepad),
            typeof(Movement),
            typeof(StateMachine),
            typeof(Flow),
            typeof(SkillTrigger),
            typeof(SkillLauncher),
            typeof(Bullet),
            typeof(Translate),
            typeof(TestAIMove)
        };
    }
}