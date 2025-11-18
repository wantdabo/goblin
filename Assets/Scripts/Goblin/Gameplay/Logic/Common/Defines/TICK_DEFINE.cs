using System;
using System.Collections.Generic;
using Goblin.Gameplay.Logic.Behaviors;
using Goblin.Gameplay.Logic.Behaviors.Sa;

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
            typeof(Detection),
            typeof(Captain),
            typeof(Gamepad),
            typeof(SkillBinding),
            typeof(Movement),
            typeof(Flow),
            typeof(HitEffect),
            typeof(SkillLauncher),
            typeof(Bullet),
            typeof(Buff),
            typeof(SilentMercy),
            typeof(Facade),
            typeof(StepEnd),
            typeof(RILSync),
        };
    }
}