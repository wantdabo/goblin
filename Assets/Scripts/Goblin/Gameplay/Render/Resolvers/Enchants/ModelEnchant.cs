using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Render.Agents;
using Goblin.Gameplay.Render.Resolvers.Common;
using Goblin.Gameplay.Render.Resolvers.States;

namespace Goblin.Gameplay.Render.Resolvers.Enchants
{
    /// <summary>
    /// 模型代理的赋能
    /// </summary>
    public class ModelEnchant : AgentEnchant<TagState>
    {
        protected override void OnRState(TagState state)
        {
            // 如果没有模型定义, 则回收模型代理, 有则创建模型代理
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