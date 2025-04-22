using Goblin.Common;
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
            var states = world.statebucket.GetStates<SpatialState>(StateType.Spatial);
            if (null == states) return;
            foreach (var state in states)
            {
                var node = world.EnsureAgent<NodeAgent>(state.actor);
                node.targetPosition = state.position;
                node.targetEuler = state.euler;
                node.targetScale = state.scale;
                node.Chase();
            }
        }
    }
}