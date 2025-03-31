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
        protected override void OnTick(FP tick)
        {
            base.OnTick(tick);
            info.elapsed += info.scale * info.tick;
            // Ticking Behavior
            List<Type> types = actor.stage.GetBehaviorTypes(actor.id);
            foreach (var type in types)
            {
                if (false == BEHAVIOR_TICKS.TYPES.Contains(type)) continue;
                if (typeof(Ticker) == type) continue;
                
                var behavior = actor.GetBehavior(type);
                behavior.Tick(info.tick);
            }
        }

        protected override void OnTickEnd()
        {
            base.OnTickEnd();
            // TickEnd Behavior
            List<Type> types = actor.stage.GetBehaviorTypes(actor.id);
            foreach (var type in types)
            {
                if (false == BEHAVIOR_TICKS.TYPES.Contains(type)) continue;
                if (typeof(Ticker) == type) continue;
                var behavior = actor.GetBehavior(type);
                behavior.TickEnd();
            }
        }
    }
}