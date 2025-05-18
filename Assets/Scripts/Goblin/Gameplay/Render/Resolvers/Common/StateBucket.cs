using System;
using System.Collections.Generic;
using Goblin.Common;
using Goblin.Core;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL.Common;
using Goblin.Gameplay.Render.Core;
using Goblin.Gameplay.Render.Resolvers.Enchants;

namespace Goblin.Gameplay.Render.Resolvers.Common
{
    /// <summary>
    /// Render-State/渲染状态的事件
    /// </summary>
    public struct RStateEvent : IEvent
    {
        /// <summary>
        /// 状态
        /// </summary>
        public State state { get; set; }
    }
    
    /// <summary>
    /// Event-State/事件状态的事件
    /// </summary>
    public struct EStateEvent : IEvent
    {
        /// <summary>
        /// 状态
        /// </summary>
        public State state { get; set; }
    }

    /// <summary>
    /// Render-State/渲染状态的事件
    /// </summary>
    /// <typeparam name="T">数据状态类型</typeparam>
    public struct RStateEvent<T> : IEvent where T : State
    {
        /// <summary>
        /// 状态
        /// </summary>
        public T state { get; set; }
    }
    
    /// <summary>
    /// Event-State/事件状态的事件
    /// </summary>
    /// <typeparam name="T">数据状态类型</typeparam>
    public struct EStateEvent<T> : IEvent where T : State
    {
        /// <summary>
        /// 状态
        /// </summary>
        public T state { get; set; }
    }

    /// <summary>
    /// 数据状态桶, 存储着渲染层的所有数据状态
    /// </summary>
    public class StateBucket : Comp
    {
        /// <summary>
        /// 世界
        /// </summary>
        public World world { get; private set; }
        /// <summary>
        /// 事件订阅派发者
        /// </summary>
        public Eventor eventor { get; private set; }
        /// <summary>
        /// 渲染状态集合
        /// </summary>
        private Dictionary<ulong, Dictionary<Type, State>> statedict { get; set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            eventor = AddComp<Eventor>();
            eventor.Create();
            
            statedict = ObjectCache.Get<Dictionary<ulong, Dictionary<Type, State>>>();
            Resolvers();
            Enchants();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            LossAllStates();
            ObjectCache.Set(statedict);
        }

        /// <summary>
        /// 初始化数据状态桶
        /// </summary>
        /// <param name="world">世界</param>
        /// <returns>初始化数据状态桶</returns>
        public StateBucket Initialize(World world)
        {
            this.world = world;

            return this;
        }

        /// <summary>
        /// 初始化渲染状态解析器
        /// </summary>
        private void Resolvers()
        {
            void Resolver<T>() where T : Resolver, new()
            {
                var resolver = AddComp<T>().Initialize(this);
                resolver.Create();
            }
            
            Resolver<AttributeResolver>();
            Resolver<SeatResolver>();
            Resolver<SpatialResolver>();
            Resolver<StageResolver>();
            Resolver<StateMachineResolver>();
            Resolver<TagResolver>();
            Resolver<TickerResolver>();
        }

        /// <summary>
        /// 初始化代理赋能
        /// </summary>
        private void Enchants()
        {
            void Enchant<T>() where T : AgentEnchant, new()
            {
                var enchant = AddComp<T>().Initialize(this);
                enchant.Create();
            }
            
            Enchant<NodeEnchant>();
            Enchant<ModelEnchant>();
            Enchant<AnimationEnchant>();
        }

        /// <summary>
        /// 清除所有的状态
        /// </summary>
        public void LossAllStates()
        {
            foreach (var states in statedict.Values)
            {
                states.Clear();
                ObjectCache.Set(states);
            }
            statedict.Clear();
        }

        /// <summary>
        /// 获取所有的状态
        /// </summary>
        /// <param name="states">数据状态列表</param>
        /// <typeparam name="T">数据状态类型</typeparam>
        /// <returns>YES/NO</returns>
        public bool SeekStates<T>(out List<T> states) where T : State, new()
        {
            states = GetStates<T>();
            
            return null != states && 0 != states.Count;
        }

        /// <summary>
        /// 获取状态
        /// </summary>
        /// <param name="actor">ActorID</param>
        /// <param name="state">数据状态</param>
        /// <typeparam name="T">数据状态类型</typeparam>
        /// <returns>YES/NO</returns>
        public bool SeekState<T>(ulong actor, out T state) where T : State
        {
            state = GetState<T>(actor);
            
            return null != state;
        }

        /// <summary>
        /// 获取所有的状态
        /// </summary>
        /// <typeparam name="T">数据状态类型</typeparam>
        /// <returns>数据状态列表</returns>
        public List<T> GetStates<T>() where T : State
        {
            var type = typeof(T);
            var states = ObjectCache.Get<List<T>>();
            foreach (var kv in statedict)
            {
                if (false == kv.Value.ContainsKey(type)) continue;
                if (false == kv.Value.TryGetValue(type, out var state)) continue;
                states.Add((T)state);
            }

            // 没有找到
            if (0 == states.Count)
            {
                ObjectCache.Set(states);

                return default;
            }

            return states;
        }

        /// <summary>
        /// 获取状态
        /// </summary>
        /// <param name="actor">ActorID</param>
        /// <typeparam name="T">数据状态类型</typeparam>
        /// <returns>数据状态</returns>
        public T GetState<T>(ulong actor) where T : State
        {
            if (false == statedict.TryGetValue(actor, out var dict)) return default;
            if (false == dict.TryGetValue(typeof(T), out var result)) return default;
            return result as T;
        }

        /// <summary>
        /// 设置状态
        /// </summary>
        /// <param name="riltype">渲染指令类型 (1, Render, 2 Event)</param>
        /// <param name="state"></param>
        /// <typeparam name="T">数据状态</typeparam>
        public void SetState<T>(byte riltype, T state) where T : State
        {
            // 事件状态
            if (RIL_DEFINE.TYPE_EVENT == riltype)
            {
                eventor.Tell(new EStateEvent { state = state });
                eventor.Tell(new EStateEvent<T> { state = state });
                
                state.Reset();
                ObjectCache.Set(state);
                
                return;
            }

            // 渲染状态
            var type = typeof(T);
            if (false == statedict.TryGetValue(state.actor, out var dict)) statedict.Add(state.actor, dict = ObjectCache.Get<Dictionary<Type, State>>());
            if (dict.TryGetValue(type, out var oldstate))
            {
                if (state.Equals(oldstate))
                {
                    state.Reset();
                    ObjectCache.Set(state);
                    
                    return;
                }
                
                oldstate.Reset();
                ObjectCache.Set(oldstate);
                dict.Remove(type);
            }
            
            dict.Add(type, state);
            eventor.Tell(new RStateEvent { state = state });
            eventor.Tell(new RStateEvent<T> { state = state });
        }
    }
}