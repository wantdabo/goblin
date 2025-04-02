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
    /// 时间驱动器
    /// </summary>
    public class Ticker : Behavior<TickerInfo>
    {
        /// <summary>
        /// 需要被 Tick 的行为类型列表
        /// </summary>
        private List<Type> ticktypes { get; set; } = new()
        {
            typeof(Gamepad),
            typeof(StateMachine)
        };
        
        protected override void OnTick(FP tick)
        {
            base.OnTick(tick);
            if (false == actor.stage.SeekBehaviorTypes(actor.id, out List<Type> types)) return;
            foreach (var type in types)
            {
                if (false == ticktypes.Contains(type)) continue;
                if (false == actor.SeekBehavior(type, out Behavior behavior)) continue;
                
                behavior.Tick(tick * info.timescale);
            }
        }

        protected override void OnTickEnd()
        {
            base.OnTickEnd();
            if (false == actor.stage.SeekBehaviorTypes(actor.id, out List<Type> types)) return;
            foreach (var type in types)
            {
                if (false == ticktypes.Contains(type)) continue;
                if (false == actor.SeekBehavior(type, out Behavior behavior)) continue;
                
                behavior.TickEnd();
            }
        }
    }
}