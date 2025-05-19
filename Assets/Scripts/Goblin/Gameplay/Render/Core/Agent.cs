using System;
using System.Collections.Generic;
using Goblin.Common;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Render.Resolvers.Common;

namespace Goblin.Gameplay.Render.Core
{
    /// <summary>
    /// 追逐状态
    /// </summary>
    public enum ChaseStatus
    {
        /// <summary>
        /// 追逐
        /// </summary>
        Chasing,
        /// <summary>
        /// 到达
        /// </summary>
        Arrived,
    }

    public class Invoker
    {
        protected ulong actor { get; set; }
        protected Delegate func { get; set; }
        
        public void Ready(ulong actor, Delegate func)
        {
            this.actor = actor;
            this.func = func;
        }
        
        public void Reset()
        {
            this.actor = 0;
            this.func = null;
        }

        public void Invoke(State state)
        {
            OnInvoke(state);
        }

        protected virtual void OnInvoke(State state) { }
    }
    
    public class Invoker<T> : Invoker where T : State
    {
        protected override void OnInvoke(State state)
        {
            base.OnInvoke(state);
            if (state.actor != actor) return;
            
            (func as Action<T>).Invoke(state as T);
        }
    }

    /// <summary>
    /// 代理
    /// </summary>
    public abstract class Agent
    {
        /// <summary>
        /// Actor
        /// </summary>
        public ulong actor { get; private set; }
        /// <summary>
        /// 世界
        /// </summary>
        public World world { get; private set; }
        /// <summary>
        /// 追逐类型
        /// </summary>
        public ChaseStatus status { get; private set; }
        /// <summary>
        /// 观察状态映射集合
        /// </summary>
        private Dictionary<Type, List<Invoker>> stateactions { get; set; }
        
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="id">ActorID</param>
        /// <param name="world">世界</param>
        public void Ready(ulong id, World world)
        {
            this.actor = id;
            this.world = world;
            this.status = ChaseStatus.Chasing;
            stateactions = ObjectCache.Get<Dictionary<Type, List<Invoker>>>();
            
            OnReady();
            Flash();
        }
        
        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            OnReset();
            
            this.actor = 0;
            this.world = null;
            this.status = ChaseStatus.Chasing;
            foreach (var kv in stateactions)
            {
                foreach (var invoker in kv.Value)
                {
                    invoker.Reset();
                    ObjectCache.Set(invoker);
                }
                
                kv.Value.Clear();
                ObjectCache.Set(kv.Value);
            }
            stateactions.Clear();
            ObjectCache.Set(stateactions);
        }
        
        /// <summary>
        /// 关心状态
        /// </summary>
        /// <param name="func">回调</param>
        /// <typeparam name="T">状态类型</typeparam>
        protected void CareState<T>(Action<T> func) where T : State
        {
            if (false == stateactions.TryGetValue(typeof(T), out var list)) stateactions.Add(typeof(T), list = ObjectCache.Get<List<Invoker>>());
            var invoker = ObjectCache.Get<Invoker<T>>();
            invoker.Ready(actor, func);
            list.Add(invoker);
        }
        
        /// <summary>
        /// 处理渲染状态
        /// </summary>
        /// <param name="state">渲染状态</param>
        public void DoState(State state)
        {
            if (false == stateactions.TryGetValue(state.GetType(), out var invokers)) return;
            foreach (var invoker in invokers) invoker.Invoke(state);
        }
        
        /// <summary>
        /// 闪现
        /// </summary>
        public void Flash()
        {
            OnFlash();
        }
        
        /// <summary>
        /// 追逐
        /// </summary>
        /// <param name="tick">tick/s</param>
        /// <param name="timescale">时间缩放</param>
        public void Chase(float tick, float timescale)
        {
            if (ChaseStatus.Arrived == status) return;
            
            OnChase(tick, timescale);
            
            if (OnArrived()) ChangeStatus(ChaseStatus.Arrived);
        }
        
        /// <summary>
        /// 更改追逐状态
        /// </summary>
        /// <param name="status">追逐状态</param>
        public void ChangeStatus(ChaseStatus status)
        {
            this.status = status;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        protected abstract void OnReady();
        /// <summary>
        /// 重置
        /// </summary>
        protected abstract void OnReset();
        /// <summary>
        /// 到达检查
        /// </summary>
        /// <returns>YES/NO</returns>
        protected virtual bool OnArrived() { return false; }
        /// <summary>
        /// 闪现
        /// </summary>
        protected virtual void OnFlash() { }
        /// <summary>
        /// 追逐
        /// </summary>
        /// <param name="tick">tick/s</param>
        /// <param name="timescale">时间缩放</param>
        protected virtual void OnChase(float tick, float timescale) { }
    }
}