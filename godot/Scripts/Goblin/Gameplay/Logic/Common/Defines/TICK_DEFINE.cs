using System;
using System.Collections.Generic;
using Goblin.Gameplay.Logic.Behaviors;
using Goblin.Gameplay.Logic.Behaviors.Sa;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.Common.Defines;

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
        typeof(Herald),
        typeof(Gamepad),
        typeof(Movement),
        typeof(StateMachine),
        typeof(Flow),
        typeof(HitEffect),
        typeof(SkillLauncher),
        typeof(Magic),
        typeof(Buff),
        typeof(SilentMercy),
        typeof(Facade),
        typeof(StepEnd),
        typeof(StageSequence),
        typeof(ProjectorSystem),
    };
}