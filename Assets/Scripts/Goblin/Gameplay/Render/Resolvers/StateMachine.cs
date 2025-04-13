using Goblin.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Render.Agents;
using Goblin.Gameplay.Render.Common.Extensions;
using Goblin.Gameplay.Render.Core;
using UnityEngine;
using Animation = Goblin.Gameplay.Render.Agents.Animation;

namespace Goblin.Gameplay.Render.Resolvers
{
    public class StateMachine : Resolver
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            world.ticker.eventor.Listen<TickEvent>(OnTick);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            world.ticker.eventor.UnListen<TickEvent>(OnTick);
        }
        
        private void OnTick(TickEvent e)
        {
            var statebundles = world.summary.GetStateBundles(RIL_DEFINE.STATE_MACHINE);
            if (null == statebundles) return;
            foreach (var bundle in statebundles)
            {
                world.EnsureAgent<Animation>(bundle.actor);
            }
        }
    }
}