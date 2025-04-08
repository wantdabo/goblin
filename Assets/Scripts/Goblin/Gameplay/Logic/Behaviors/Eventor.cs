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
        /// <summary>
        /// 注销事件监听
        /// </summary>
        /// <typeparam name="T">事件的结构体</typeparam>
        /// <param name="target">Actor</param>
        /// <param name="func">事件的回调</param>
        public void UnListen<T>(Actor target, Action<T> func) where T : IEvent
        {
            if (false == info.eventdict.TryGetValue(actor.id, out var dict)) return;
            if (false == dict.TryGetValue(typeof(T), out var funcs)) return;
            funcs.Remove(func);
        }
        
        /// <summary>
        /// 注册事件监听
        /// </summary>
        /// <typeparam name="T">事件的结构体</typeparam>
        /// <param name="target">Behavior</param>
        /// <param name="func">事件的回调</param>
        public void Listen<T>(Actor target, Action<T> func) where T : IEvent
        {
            if (false == info.eventdict.TryGetValue(actor.id, out var dict)) info.eventdict.Add(actor.id, dict = ObjectCache.Get<Dictionary<Type, List<Delegate>>>());
            if (false == dict.TryGetValue(typeof(T), out var funcs)) dict.Add(typeof(T), funcs = ObjectCache.Get<List<Delegate>>());
            if (funcs.Contains(func)) return;
            funcs.Add(func);
        }

        /// <summary>
        /// 派发事件
        /// </summary>
        /// <typeparam name="T">事件的结构体</typeparam>
        /// <param name="e">事件的参数</param>
        public void Tell<T>(T e = default) where T : IEvent
        {
            if (false == stage.SeekBehaviors<Eventor>(out var eventors)) return;
            foreach (var eventor in eventors) eventor.Call(actor.id, e);
            eventors.Clear();
            ObjectCache.Set(eventors);
        }
        
        /// <summary>
        /// 派发事件
        /// </summary>
        /// <param name="actor">ActorID</param>
        /// <param name="e">事件的参数</param>
        /// <typeparam name="T">事件的结构体</typeparam>
        public void Call<T>(ulong actor, T e = default) where T : IEvent
        {
            if (false == info.eventdict.TryGetValue(actor, out var dict)) return;
            if (false == dict.TryGetValue(typeof(T), out var funcs)) return;
            foreach (var func in funcs) (func as Action<T>)?.Invoke(e);
        }
    }
}