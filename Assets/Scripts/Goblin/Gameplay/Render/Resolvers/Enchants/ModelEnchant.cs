using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Render.Agents;
using Goblin.Gameplay.Render.Resolvers.Common;

namespace Goblin.Gameplay.Render.Resolvers.Enchants
{
    /// <summary>
    /// 模型代理的赋能
    /// </summary>
    public class ModelEnchant : AgentEnchant<RIL_TAG>
    {
        protected override void OnRIL(RIL_TAG ril)
        {
            // 如果没有模型定义, 则回收模型代理, 有则创建模型代理
            if (ril.tags.ContainsKey(TAG_DEFINE.MODEL))
            {
                rilbucket.world.EnsureAgent<ModelAgent>(ril.actor);
            }
            else
            {
                var agent = rilbucket.world.EnsureAgent<ModelAgent>(ril.actor);
                if (null == agent) return;
                rilbucket.world.RmvAgent(agent);
            }
        }
    }
}