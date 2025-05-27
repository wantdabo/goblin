using System;
using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.Behaviors
{
    /// <summary>
    /// 事件接口
    /// </summary>
    public interface IEvent { }
    
    /// <summary>
    /// 事件订阅派发者
    /// </summary>
    public class Eventor : Behavior<EventorInfo>
    {
        private Dictionary<Type, List<(uint index, Delegate action)>> eventdict { get; set; }
        
        protected override void OnAssemble()
        {
            base.OnAssemble();
            eventdict = ObjectCache.Ensure<Dictionary<Type, List<(uint index, Delegate)>>>();
        }

        protected override void OnDisassemble()
        {
            base.OnDisassemble();
            if (null == eventdict) return;
            foreach (var kv in eventdict)
            {
                kv.Value.Clear();
                ObjectCache.Set(kv.Value);
            }
            eventdict.Clear();
            ObjectCache.Set(eventdict);
        }
        
        /// <summary>
        /// 注销事件监听
        /// </summary>
        /// <typeparam name="T">事件的结构体</typeparam>
        /// <param name="func">事件的回调</param>
        public void UnListen<T>(Behavior behavior, Action<T> func) where T : IEvent
        {
            if (false == eventdict.TryGetValue(typeof(T), out var funcs)) return;
            var behaviorhash = behavior.GetHashCode();
            if (false == info.indexes.TryGetValue((behaviorhash, behavior.actor), out var index)) return;
            funcs.Remove((index, func));
            info.indexes.Remove((behaviorhash, behavior.actor));
        }
        
        /// <summary>
        /// 注册事件监听
        /// </summary>
        /// <typeparam name="T">事件的结构体</typeparam>
        /// <param name="func">事件的回调</param>
        public void Listen<T>(Behavior behavior, Action<T> func) where T : IEvent
        {
            if (false == eventdict.TryGetValue(typeof(T), out var funcs))
            {
                funcs = ObjectCache.Ensure<List<(uint index, Delegate)>>();
                eventdict.Add(typeof(T), funcs);
            }

            var behaviorhash = behavior.GetHashCode();
            bool notsort = false;
            if (false == info.indexes.TryGetValue((behaviorhash, behavior.actor), out var index))
            {
                info.increment = info.increment + 1;
                index = info.increment;
                info.indexes.Add((behaviorhash, behavior.actor), index);
                notsort = true;
            }

            if (funcs.Contains((index, func))) return;
            funcs.Add((index, func));
            
            if (notsort) return;
            funcs.Sort((funca, funcb) => funca.index.CompareTo(funcb.index));
        }
        
        /// <summary>
        /// 派发事件
        /// </summary>
        /// <typeparam name="T">事件的结构体</typeparam>
        /// <param name="e">事件的参数</param>
        public void Tell<T>(T e = default) where T : IEvent
        {
            if (null == eventdict) return;
            if (false == eventdict.TryGetValue(typeof(T), out var funcs)) return;
            for (int i = funcs.Count - 1; i >= 0; i--) (funcs[i].action as Action<T>).Invoke(e);
        }
    }
}