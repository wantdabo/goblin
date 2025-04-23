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
        private Dictionary<ulong, Dictionary<StateType, State>> statedict { get; set; }
        private List<Resolver> resolvers { get; set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            statedict = ObjectCache.Get<Dictionary<ulong, Dictionary<StateType, State>>>();
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
        
        public bool GetStates<T>(StateType st, out List<T> states) where T : State, new()
        {
            states= ObjectCache.Get<List<T>>();
            foreach (var kv in statedict)
            {
                if (false == kv.Value.ContainsKey(st)) continue;
                if (false == kv.Value.TryGetValue(st, out var state)) continue;
                states.Add((T)state);
            }

            // 没有找到
            if (0 == states.Count)
            {
                ObjectCache.Set(states);

                return false;
            }

            return true;
        }

        public bool GetState<T>(ulong actor, StateType st, out T state) where T : State
        {
            state = default;
            if (false == statedict.TryGetValue(actor, out var dict)) return false;
            if (false == dict.TryGetValue(st, out var result)) return false;
            state = (T)result;
            
            return true;
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

            if (false == statedict.TryGetValue(state.actor, out var dict)) statedict.Add(state.actor, dict = ObjectCache.Get<Dictionary<StateType, State>>());
            if (dict.TryGetValue(state.type, out var oldstate))
            {
                oldstate.Reset();
                ObjectCache.Set(oldstate);
                dict.Remove(state.type);
            }

            dict.Add(state.type, state);
        }
    }
}