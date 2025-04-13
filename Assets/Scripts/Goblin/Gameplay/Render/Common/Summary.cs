using System.Collections.Generic;
using Goblin.Common;
using Goblin.Core;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.RIL.Common;
using Goblin.Gameplay.Render.Core;

namespace Goblin.Gameplay.Render.Common
{
    public struct ABStateInfo
    {
        public ABStateInfo(ulong actor, IRIL ril)
        {
            this.actor = actor;
            this.ril = ril;
        }

        public ulong actor { get; set; }
        public IRIL ril { get; set; }
    }
    
    public class Summary : Comp
    {
        private World world { get; set; }
        private Dictionary<ulong, Dictionary<ushort, ABStateInfo>> statedict { get; set; }
        private Dictionary<ushort, List<ABStateInfo>> statebundles { get; set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            world.eventor.Listen<RILEvent>(OnRIL);
            world.ticker.eventor.Listen<LateTickEvent>(OnLateTick);
            
            statedict = ObjectCache.Get<Dictionary<ulong, Dictionary<ushort, ABStateInfo>>>();
            statebundles = ObjectCache.Get<Dictionary<ushort, List<ABStateInfo>>>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            world.eventor.UnListen<RILEvent>(OnRIL);
            world.ticker.eventor.UnListen<LateTickEvent>(OnLateTick);
            
            foreach (var states in statedict.Values)
            {
                states.Clear();
                ObjectCache.Set(states);
            }
            statedict.Clear();
            ObjectCache.Set(statedict);
            
            foreach (var states in statebundles.Values)
            {
                states.Clear();
                ObjectCache.Set(states);
            }
            statebundles.Clear();
            ObjectCache.Set(statebundles);
        }

        public void Initialize(World world)
        {
            this.world = world;
        }
        
        public List<ABStateInfo> GetStateBundles(ushort id)
        {
            if (false == statebundles.TryGetValue(id, out var states)) return default;
            
            return states;
        }

        public ABStateInfo GetState(ulong actor, ushort id)
        {
            if (false == statedict.TryGetValue(actor, out var statemap)) return default;
            if (false == statemap.TryGetValue(id, out var state)) return default;

            return state;
        }

        private void OnRIL(RILEvent e)
        {
            if (false == statedict.TryGetValue(e.state.actor, out var statemap))
            {
                statedict.Add(e.state.actor, statemap = ObjectCache.Get<Dictionary<ushort, ABStateInfo>>());
            }
            
            if (statemap.ContainsKey(e.state.ril.id))
            {
                statemap.Remove(e.state.ril.id);
            }
            statemap.Add(e.state.ril.id, e.state);
        }

        private void OnLateTick(LateTickEvent e)
        {
            foreach (var kv in statebundles)
            {
                kv.Value.Clear();
                ObjectCache.Set(kv.Value);
            }
            statebundles.Clear();

            foreach (var statemap in statedict.Values)
            {
                foreach (var state in statemap.Values)
                {
                    if (false == statebundles.TryGetValue(state.ril.id, out var states))
                    {
                        statebundles.Add(state.ril.id, states = ObjectCache.Get<List<ABStateInfo>>());
                    }
                    
                    states.Add(state);
                }
            }
        }
    }
}