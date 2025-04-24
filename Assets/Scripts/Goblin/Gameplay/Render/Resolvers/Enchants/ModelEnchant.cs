using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Render.Agents;
using Goblin.Gameplay.Render.Resolvers.Common;
using Goblin.Gameplay.Render.Resolvers.States;

namespace Goblin.Gameplay.Render.Resolvers.Enchants
{
    public class ModelEnchant : AgentEnchant<TagState>
    {
        protected override void OnRState(TagState state)
        {
            if (state.tags.ContainsKey(TAG_DEFINE.MODEL))
            {
                statebucket.world.EnsureAgent<ModelAgent>(state.actor);
            }
            else
            {
                var agent = statebucket.world.EnsureAgent<ModelAgent>(state.actor);
                if (null == agent) return;
                statebucket.world.RmvAgent(agent);
            }
        }
    }
}