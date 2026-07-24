using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Render.Agents;
using Goblin.Gameplay.Render.Resolvers.Common;

namespace Goblin.Gameplay.Render.Resolvers.Enchants;

public class SpatialEnchant : AgentEnchant<RIL_SPATIAL>
{
    protected override void OnRIL(RIL_SPATIAL ril)
    {
        rilbucket.world.EnsureAgent<SpatialAgent>(ril.actor);
    }
}