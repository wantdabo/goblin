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
    public class Ticking : Attribute
    {
    }
    
    /// <summary>
    /// 驱动器
    /// </summary>
    public class Ticker : Behavior<TickerInfo>
    {
        /// <summary>
        /// 驱动
        /// </summary>
        public void Tick()
        {
            info.frame++;
            info.elapsed += info.tick;
            
            if (info.breakframes > 0)
            {
                info.breakframes--;
                return;
            }

            // Collection Ticking Behavior && Tick
            List<Type> types = actor.stage.GetBehaviors(actor.id);
            var tbehaviors = ObjectCache.Get<List<Behavior>>();
            foreach (var type in types)
            {
                if (null == type.GetCustomAttribute<Ticking>()) continue;
                var behavior = actor.GetBehavior(type);
                tbehaviors.Add(behavior);
                behavior.Tick(info.tick);
            }
            // LateTick
            foreach (var behavior in tbehaviors) behavior.LateTick(info.tick);
            
            tbehaviors.Clear();
            ObjectCache.Set(tbehaviors);
        }
    }
}