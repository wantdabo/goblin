using System;
using System.Collections.Generic;
using System.Reflection;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Behaviors
{
    [AttributeUsage(AttributeTargets.Class)]
    public class Ticking : System.Attribute
    {
    }
    
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
                if (null == type.GetCustomAttribute<Ticking>()) continue;
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
                if (null == type.GetCustomAttribute<Ticking>()) continue;
                var behavior = actor.GetBehavior(type);
                behavior.TickEnd();
            }
        }
    }
}