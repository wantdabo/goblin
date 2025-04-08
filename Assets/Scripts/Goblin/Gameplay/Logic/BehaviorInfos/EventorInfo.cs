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
        public Dictionary<ulong, Dictionary<Type, List<Delegate>>> eventdict { get; set; }

        protected override void OnReady()
        {
            eventdict = ObjectCache.Get<Dictionary<ulong, Dictionary<Type, List<Delegate>>>>();
        }

        protected override void OnReset()
        {
            foreach (var kv in eventdict)
            {
                foreach (var kv2 in kv.Value)
                {
                    kv2.Value.Clear();
                    ObjectCache.Set(kv2.Value);
                }
                kv.Value.Clear();
                ObjectCache.Set(kv.Value);
            }
            eventdict.Clear();
            ObjectCache.Set(eventdict);
        }

        protected override BehaviorInfo OnClone()
        {
            throw new NotImplementedException();
        }
    }
}