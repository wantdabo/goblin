using System;
using System.Collections.Generic;
using System.Reflection;
using Goblin.Gameplay.Common.Defines;
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
            info.frame++;
            info.elapsed += info.tick;
            info.breaked = info.breakframes > 0;
            info.breakframes--;
            FPMath.Clamp(info.breakframes, 0, uint.MaxValue);
            
            if (info.breaked) return;

            // Ticking Behavior
            List<Type> types = actor.stage.GetBehaviors(actor.id);
            foreach (var type in types)
            {
                if (null == type.GetCustomAttribute<Ticking>()) continue;
                var behavior = actor.GetBehavior(type);
                behavior.Tick(info.tick);
            }
        }

        protected override void OnLateTick(FP tick)
        {
            base.OnLateTick(tick);
            if (info.breaked) return;
            
            // LateTicking Behavior
            List<Type> types = actor.stage.GetBehaviors(actor.id);
            foreach (var type in types)
            {
                if (null == type.GetCustomAttribute<Ticking>()) continue;
                var behavior = actor.GetBehavior(type);
                behavior.LateTick(info.tick);
            }
        }
    }
}