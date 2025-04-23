using Goblin.Common;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Render.Agents;
using Goblin.Gameplay.Render.Common.Extensions;
using Goblin.Gameplay.Render.Core;
using Goblin.Gameplay.Render.Resolvers.Common;
using Goblin.Gameplay.Render.Resolvers.States;

namespace Goblin.Gameplay.Render.Batches
{
    public class SpatialBatch : Batch
    {
        protected override void OnTick(TickEvent e)
        {
            if (false == world.statebucket.GetStates<SpatialState>(out var states)) return;
            foreach (var state in states)
            {
                var node = world.EnsureAgent<NodeAgent>(state.actor);
                node.targetPosition = state.position;
                node.targetEuler = state.euler;
                node.targetScale = state.scale;
            }
            
            states.Clear();
            ObjectCache.Set(states);
        }
    }
}