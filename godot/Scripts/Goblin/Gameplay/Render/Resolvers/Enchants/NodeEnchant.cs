using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Render.Agents;
using Goblin.Gameplay.Render.Resolvers.Common;

namespace Goblin.Gameplay.Render.Resolvers.Enchants
{
    /// <summary>
    /// 节点代理赋能
    /// </summary>
    public class NodeEnchant : AgentEnchant<RIL_SPATIAL>
    {
        protected override void OnRIL(RIL_SPATIAL ril)
        {
            rilbucket.world.EnsureAgent<NodeAgent>(ril.actor);
        }
    }
}