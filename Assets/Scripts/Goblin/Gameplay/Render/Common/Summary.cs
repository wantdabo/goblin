using System.Collections.Generic;
using Goblin.Core;
using Goblin.Gameplay.Logic.RIL.Common;
using Goblin.Gameplay.Render.Core;

namespace Goblin.Gameplay.Render.Common
{
    public struct ABStateInfo
    {
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
            statedict = world.engine.pool.Get<Dictionary<ulong, Dictionary<ushort, ABStateInfo>>>();
            statebundles = world.engine.pool.Get<Dictionary<ushort, List<ABStateInfo>>>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            world.eventor.UnListen<RILEvent>(OnRIL);
            
            foreach (var states in statedict.Values)
            {
                states.Clear();
                world.engine.pool.Set(states);
            }
            statedict.Clear();
            world.engine.pool.Set(statedict);
            
            foreach (var states in statebundles.Values)
            {
                states.Clear();
                world.engine.pool.Set(states);
            }
            statebundles.Clear();
            world.engine.pool.Set(statebundles);
        }

        public void Initialize(World world)
        {
            this.world = world;
        }

        private void OnRIL(RILEvent e)
        {
            if (false == statedict.TryGetValue(e.state.actor, out var statemap))
            {
                statedict.Add(e.state.actor, statemap = world.engine.pool.Get<Dictionary<ushort, ABStateInfo>>());
            }
            
            if (false == statemap.TryGetValue(e.state.ril.id, out var state))
            {
                statemap.Remove(e.state.ril.id);
            }
            statemap.Add(e.state.ril.id, state);
        }
    }
}