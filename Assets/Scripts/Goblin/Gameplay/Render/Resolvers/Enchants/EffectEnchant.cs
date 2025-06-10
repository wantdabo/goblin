using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Render.Agents;
using Goblin.Gameplay.Render.Resolvers.Common;

namespace Goblin.Gameplay.Render.Resolvers.Enchants
{
    /// <summary>
    /// 外观特效代理的赋能
    /// </summary>
    public class EffectEnchant : AgentEnchant<RIL_FACADE_EFFECT>
    {
        protected override void OnRIL(RIL_FACADE_EFFECT ril)
        {
            // 如果没有特效定义, 则回收特效代理, 有则创建特效代理
            if (ril.effectdict.Count > 0)
            {
                rilbucket.world.EnsureAgent<EffectAgent>(ril.actor);
            }
            else
            {
                RecycleAgent(ril.actor);
            }
        }
        
        private void RecycleAgent(ulong actor)
        {
            var agent = rilbucket.world.GetAgent<EffectAgent>(actor);
            if (null == agent) return;
            rilbucket.world.RmvAgent(agent);
        }
    }
}