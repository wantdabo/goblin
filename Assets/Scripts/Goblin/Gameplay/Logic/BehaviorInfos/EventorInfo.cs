using System;
using System.Collections.Generic;
using System.Reflection;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.BehaviorInfos
{
    /// <summary>
    /// 事件订阅派发者信息
    /// </summary>
    public class EventorInfo : BehaviorInfo
    {
        /// <summary>
        /// 事件字典, 键为事件 ID, 值为事件类型和方法列表
        /// </summary>
        public Dictionary<ulong, Dictionary<Type, List<MethodInfo>>> eventdict { get; set; }

        protected override void OnReady()
        {
            eventdict = ObjectCache.Get<Dictionary<ulong, Dictionary<Type, List<MethodInfo>>>>();
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
            var clone = ObjectCache.Get<EventorInfo>();
            clone.Ready(id);
            foreach (var kv in eventdict)
            {
                var dict = ObjectCache.Get<Dictionary<Type, List<MethodInfo>>>();
                foreach (var kv2 in kv.Value)
                {
                    var funcs = ObjectCache.Get<List<MethodInfo>>();
                    foreach (var func in kv2.Value)
                    {
                        funcs.Add(func);
                    }
                    dict.Add(kv2.Key, funcs);
                }
                clone.eventdict.Add(kv.Key, dict);
            }

            return clone;
        }
    }
}