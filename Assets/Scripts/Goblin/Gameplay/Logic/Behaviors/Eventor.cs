using System;
using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
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
        /// <summary>
        /// 注销事件监听
        /// </summary>
        /// <typeparam name="T">事件的结构体</typeparam>
        /// <param name="func">事件的回调</param>
        public void UnListen<T>(Action<T> func) where T : IEvent
        {
            // if (false == eventdict.TryGetValue(typeof(T), out var funcs)) return;
            // funcs.Remove(func);
        }

        /// <summary>
        /// 注册事件监听
        /// </summary>
        /// <typeparam name="T">事件的结构体</typeparam>
        /// <param name="func">事件的回调</param>
        public void Listen<T>(Action<T> func) where T : IEvent
        {
            // if (null == eventdict) eventdict = new Dictionary<Type, List<Delegate>>();
            //
            // if (false == eventdict.TryGetValue(typeof(T), out var funcs))
            // {
            //     funcs = new List<Delegate>();
            //     eventdict.Add(typeof(T), funcs);
            // }
            // if (funcs.Contains(func)) return;
            //
            // funcs.Add(func);
        }

        /// <summary>
        /// 派发事件
        /// </summary>
        /// <typeparam name="T">事件的结构体</typeparam>
        /// <param name="e">事件的参数</param>
        public void Tell<T>(T e = default) where T : IEvent
        {
            // if (null == eventdict) return;
            // if (false == eventdict.TryGetValue(typeof(T), out var funcs)) return;
            // for (int i = funcs.Count - 1; i >= 0; i--) (funcs[i] as Action<T>).Invoke(e);
        }
    }
}