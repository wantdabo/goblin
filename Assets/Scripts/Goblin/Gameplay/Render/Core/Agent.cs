using System;
using System.Collections.Generic;
using Goblin.Common;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.RIL.Common;
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
            actor = 0;
            func = null;
        }

        public void Invoke(IRIL ril)
        {
            OnInvoke(ril);
        }

        protected virtual void OnInvoke(IRIL ril) { }
    }
    
    public class Invoker<T> : Invoker where T : IRIL
    {
        protected override void OnInvoke(IRIL ril)
        {
            base.OnInvoke(ril);
            if (ril.actor != actor) return;
            
            (func as Action<T>).Invoke(ril as T);
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
        private Dictionary<Type, List<Invoker>> rilactions { get; set; }
        
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="id">ActorID</param>
        /// <param name="world">世界</param>
        public void Ready(ulong id, World world)
        {
            actor = id;
            this.world = world;
            status = ChaseStatus.Chasing;
            rilactions = ObjectCache.Ensure<Dictionary<Type, List<Invoker>>>();
            
            OnReady();
            Flash();
        }
        
        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            OnReset();
            
            actor = 0;
            world = null;
            status = ChaseStatus.Chasing;
            foreach (var kv in rilactions)
            {
                foreach (var invoker in kv.Value)
                {
                    invoker.Reset();
                    ObjectCache.Set(invoker);
                }
                
                kv.Value.Clear();
                ObjectCache.Set(kv.Value);
            }
            rilactions.Clear();
            ObjectCache.Set(rilactions);
        }
        
        /// <summary>
        /// 处理渲染状态
        /// </summary>
        /// <param name="ril">渲染指令</param>
        public void DoRIL(IRIL ril)
        {
            if (false == rilactions.TryGetValue(ril.GetType(), out var invokers)) return;
            foreach (var invoker in invokers) invoker.Invoke(ril);
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
        /// 关心状态
        /// </summary>
        /// <param name="func">回调</param>
        /// <typeparam name="T">状态类型</typeparam>
        protected void WatchRIL<T>(Action<T> func) where T : IRIL
        {
            if (false == rilactions.TryGetValue(typeof(T), out var list)) rilactions.Add(typeof(T), list = ObjectCache.Ensure<List<Invoker>>());
            var invoker = ObjectCache.Ensure<Invoker<T>>();
            invoker.Ready(actor, func);
            list.Add(invoker);
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