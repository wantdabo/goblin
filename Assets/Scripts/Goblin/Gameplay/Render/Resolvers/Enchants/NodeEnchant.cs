using Goblin.Gameplay.Render.Agents;
using Goblin.Gameplay.Render.Resolvers.Common;
using Goblin.Gameplay.Render.Resolvers.States;

namespace Goblin.Gameplay.Render.Resolvers.Enchants
{
    public class NodeEnchant : AgentEnchant<SpatialState>
    {
        protected override void OnRState(SpatialState state)
        {
            statebucket.world.EnsureAgent<NodeAgent>(state.actor);
        }
    }
}