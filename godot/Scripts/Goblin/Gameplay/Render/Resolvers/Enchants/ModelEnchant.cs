using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Render.Agents;
using Goblin.Gameplay.Render.Resolvers.Common;

namespace Goblin.Gameplay.Render.Resolvers.Enchants
{
    /// <summary>
    /// 模型代理的赋能
    /// </summary>
    public class ModelEnchant : AgentEnchant<RIL_FACADE_MODEL>
    {
        protected override void OnRIL(RIL_FACADE_MODEL ril)
        {
            // 如果没有模型定义, 则回收模型代理, 有则创建模型代理
            if (ril.model > 0)
            {
                rilbucket.world.EnsureAgent<ModelAgent>(ril.actor);
            }
            else
            {
                RecycleAgent(ril.actor);
            }
        }
        
        protected override void OnLossRIL(RIL_LOSS ril)
        {
            base.OnLossRIL(ril);
            RecycleAgent(ril.actor);
        }
        
        private void RecycleAgent(ulong actor)
        {
            var agent = rilbucket.world.GetAgent<ModelAgent>(actor);
            if (null == agent) return;
            rilbucket.world.RmvAgent(agent);
        }
    }
}