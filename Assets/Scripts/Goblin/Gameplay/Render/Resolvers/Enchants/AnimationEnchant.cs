using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Render.Agents;
using Goblin.Gameplay.Render.Core;
using Goblin.Gameplay.Render.Resolvers.Common;
using Goblin.Gameplay.Render.Resolvers.States;

namespace Goblin.Gameplay.Render.Resolvers.Enchants
{
    /// <summary>
    /// 动画代理赋能
    /// </summary>
    public class AnimationEnchant : AgentEnchant<TagState>
    {
        protected override void OnRState(TagState state)
        {
            // 如果没有模型定义, 则回收动画代理
            if (false == state.tags.TryGetValue(TAG_DEFINE.MODEL, out var model)) 
            {
                RecycleAgent(state.actor);
                return;
            }
            
            // 如果模型定义没有动画配置, 则回收动画代理
            var modelinfo = statebucket.engine.cfg.location.ModelInfos.Get(model);
            if (null == modelinfo || string.IsNullOrEmpty(modelinfo.Animation))
            {
                RecycleAgent(state.actor);
                return;
            }
            
            // 如果模型定义有动画配置, 则创建动画代理
            statebucket.world.EnsureAgent<AnimationAgent>(state.actor);
        }

        /// <summary>
        /// 回收动画代理
        /// </summary>
        /// <param name="actor">ActorID</param>
        private void RecycleAgent(ulong actor)
        {
            var agent = statebucket.world.GetAgent<AnimationAgent>(actor);
            if (null == agent) return;
            statebucket.world.RmvAgent(agent);
        }
    }
}