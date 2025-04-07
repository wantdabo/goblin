using System;
using System.Collections.Generic;
using Goblin.Gameplay.Logic.Batchs;
using Goblin.Gameplay.Logic.Behaviors;

namespace Goblin.Gameplay.Logic.Common.Defines
{
    /// <summary>
    /// Tick 定义
    /// </summary>
    public class TICK_DEFINE
    {
        /// <summary>
        /// 行为 Tick
        /// </summary>
        public const byte BEHAVIOR = 0;
        /// <summary>
        /// 批量 Tick
        /// </summary>
        public const byte BATCH = 1;
        
        /// <summary>
        /// Tick 时序
        /// </summary>
        public static List<(byte ticktype, Type type)> TICK_TYPE_LIST { get; set; } = new()
        {
            (BEHAVIOR, typeof(Gamepad)),
            (BEHAVIOR, typeof(Movement)),
            (BEHAVIOR, typeof(StateMachine)),
            (BATCH, typeof(Translator))
        };
    }
}