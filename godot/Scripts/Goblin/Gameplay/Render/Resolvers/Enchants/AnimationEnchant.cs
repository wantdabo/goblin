using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Render.Agents;
using Goblin.Gameplay.Render.Core;
using Goblin.Gameplay.Render.Resolvers.Common;

namespace Goblin.Gameplay.Render.Resolvers.Enchants
{
    /// <summary>
    /// 动画代理赋能
    /// </summary>
    public class AnimationEnchant : AgentEnchant<RIL_FACADE_ANIMATION>
    {
        protected override void OnRIL(RIL_FACADE_ANIMATION ril)
        {
            // 如果没有模型定义, 则回收动画代理
            if (false == rilbucket.SeekRIL(ril.actor, out RIL_FACADE_MODEL facademodel) || 0 >= facademodel.model)
            {
                RecycleAgent(ril.actor);
                return;
            }

            // 如果模型定义没有动画配置, 则回收动画代理
            if (false == engine.cfg.location.ModelInfos.TryGetValue(facademodel.model, out var modelinfo)) return;
            if (string.IsNullOrEmpty(modelinfo.Animation))
            {
                RecycleAgent(ril.actor);
                return;
            }
            
            // 如果模型定义有动画配置, 则创建动画代理
            rilbucket.world.EnsureAgent<AnimationAgent>(ril.actor);
        }

        protected override void OnLossRIL(RIL_LOSS ril)
        {
            base.OnLossRIL(ril);
            RecycleAgent(ril.actor);
        }

        /// <summary>
        /// 回收动画代理
        /// </summary>
        /// <param name="actor">ActorID</param>
        private void RecycleAgent(ulong actor)
        {
            var agent = rilbucket.world.GetAgent<AnimationAgent>(actor);
            if (null == agent) return;
            rilbucket.world.RmvAgent(agent);
        }
    }
}