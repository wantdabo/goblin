using System;
using System.Collections.Generic;
using System.Reflection;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Behaviors
{
    /// <summary>
    /// 驱动器
    /// </summary>
    public class Ticker : Behavior<TickerInfo>
    {
        private static List<Type> ttypes { get; set; } = new()
        {
            typeof(Gamepad),
            typeof(StateMachine)
        };
        
        protected override void OnTick(FP tick)
        {
            base.OnTick(tick);
            info.elapsed += info.scale * info.tick;
            // Ticking Behavior
            if (false == actor.stage.SeekBehaviorTypes(actor.id, out List<Type> types)) return;
            foreach (var type in types)
            {
                if (false == ttypes.Contains(type)) continue;
                if (typeof(Ticker) == type) continue;
                if (false == actor.SeekBehavior(type, out Behavior behavior)) continue;
                behavior.Tick(info.tick);
            }
        }

        protected override void OnTickEnd()
        {
            base.OnTickEnd();
            // TickEnd Behavior
            if (false == actor.stage.SeekBehaviorTypes(actor.id, out List<Type> types)) return;
            foreach (var type in types)
            {
                if (false == ttypes.Contains(type)) continue;
                if (typeof(Ticker) == type) continue;
                if (false == actor.SeekBehavior(type, out Behavior behavior)) continue;
                behavior.TickEnd();
            }
        }
    }
}