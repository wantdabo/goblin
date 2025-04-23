using System;
using System.Collections.Generic;
using Goblin.Common;
using Goblin.Core;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL.Common;
using Goblin.Gameplay.Render.Core;

namespace Goblin.Gameplay.Render.Resolvers.Common
{
    public class StateBucket : Comp
    {
        public World world { get; private set; }
        private Dictionary<ulong, Dictionary<Type, State>> statedict { get; set; }
        private List<Resolver> resolvers { get; set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            statedict = ObjectCache.Get<Dictionary<ulong, Dictionary<Type, State>>>();
            resolvers = ObjectCache.Get<List<Resolver>>();
            Resolvers();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            foreach (var states in statedict.Values)
            {
                states.Clear();
                ObjectCache.Set(states);
            }
            statedict.Clear();
            ObjectCache.Set(statedict);
            
            resolvers.Clear();
            ObjectCache.Set(resolvers);
        }

        public StateBucket Initialize(World world)
        {
            this.world = world;

            return this;
        }

        private void Resolvers()
        {
            void AddResolver<T>() where T : Resolver, new()
            {
                var resolver = AddComp<T>().Initialize(this);
                resolvers.Add(resolver);
            }
            
            AddResolver<AttributeResolver>();
            AddResolver<SeatResolver>();
            AddResolver<SpatialResolver>();
            AddResolver<StageResolver>();
            AddResolver<StateMachineResolver>();
            AddResolver<TagResolver>();
            AddResolver<TickerResolver>();
            foreach (var resolver in resolvers) resolver.Create();
        }
        
        public bool SeekStates<T>(out List<T> states) where T : State, new()
        {
            states = GetStates<T>();
            
            return null != states && 0 != states.Count;
        }

        public bool SeekState<T>(ulong actor, out T state) where T : State
        {
            state = GetState<T>(actor);
            
            return null != state;
        }

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

        public T GetState<T>(ulong actor) where T : State
        {
            if (false == statedict.TryGetValue(actor, out var dict)) return default;
            if (false == dict.TryGetValue(typeof(T), out var result)) return default;
            return result as T;
        }

        public void SetState(State state)
        {
            if (RIL_DEFINE.TYPE_EVENT == state.rstype)
            {
                // TODO 抛事件
                state.Reset();
                ObjectCache.Set(state);
                
                return;
            }

            var type = state.GetType();
            if (false == statedict.TryGetValue(state.actor, out var dict)) statedict.Add(state.actor, dict = ObjectCache.Get<Dictionary<Type, State>>());
            if (dict.TryGetValue(type, out var oldstate))
            {
                oldstate.Reset();
                ObjectCache.Set(oldstate);
                dict.Remove(type);
            }

            dict.Add(type, state);
        }
    }
}