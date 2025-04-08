using System;
using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.BehaviorInfos
{
    /// <summary>
    /// 事件订阅派发者信息
    /// </summary>
    public class EventorInfo : BehaviorInfo
    {
        public Dictionary<Type, List<(long sort, Delegate func)>> eventdict { get; set; }
        
        protected override void OnReady()
        {
            eventdict = ObjectCache.Get<Dictionary<Type, List<(long sort, Delegate func)>>>();
        }

        protected override void OnReset()
        {
            foreach (var kv in eventdict)
            {
                kv.Value.Clear();
                ObjectCache.Set(kv.Value);
            }
            eventdict.Clear();
            ObjectCache.Set(eventdict);
        }
    }
}