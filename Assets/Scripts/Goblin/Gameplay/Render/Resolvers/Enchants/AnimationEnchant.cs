using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Render.Agents;
using Goblin.Gameplay.Render.Core;
using Goblin.Gameplay.Render.Resolvers.Common;
using Goblin.Gameplay.Render.Resolvers.States;

namespace Goblin.Gameplay.Render.Resolvers.Enchants
{
    public class AnimationEnchant : AgentEnchant<TagState>
    {
        protected override void OnRState(TagState state)
        {
            if (false == state.tags.TryGetValue(TAG_DEFINE.MODEL, out var model)) 
            {
                RecycleAgent(state.actor);
                return;
            }
            
            var modelinfo = statebucket.engine.cfg.location.ModelInfos.Get(model);
            if (null == modelinfo || string.IsNullOrEmpty(modelinfo.Animation))
            {
                RecycleAgent(state.actor);
                return;
            }
            
            statebucket.world.EnsureAgent<AnimationAgent>(state.actor);
        }

        private void RecycleAgent(ulong actor)
        {
            var agent = statebucket.world.GetAgent<AnimationAgent>(actor);
            if (null == agent) return;
            statebucket.world.RmvAgent(agent);
        }
    }
}