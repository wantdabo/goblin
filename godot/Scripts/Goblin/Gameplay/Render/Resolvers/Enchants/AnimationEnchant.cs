using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Render.Agents;
using Goblin.Gameplay.Render.Core;
using Goblin.Gameplay.Render.Resolvers.Common;

namespace Goblin.Gameplay.Render.Resolvers.Enchants;

/// <summary>
/// 动画代理赋能
/// 按 ModelInfo.Type 路由: primitive → PrimitiveAnimAgent, 其他 → AnimationAgent(需 AnimationConfig)
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

        var modelinfo = engine.cfg.location.ModelInfos.GetOrDefault(facademodel.model);
        if (null == modelinfo) return;

        if ("primitive" == modelinfo.Type)
        {
            RmvAnimationAgent(ril.actor);
            rilbucket.world.EnsureAgent<PrimitiveAnimAgent>(ril.actor);
            return;
        }

        // glb 模型需要动画配置
        if (string.IsNullOrEmpty(modelinfo.Animation))
        {
            RecycleAgent(ril.actor);
            return;
        }

        RmvPrimitiveAnimAgent(ril.actor);
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
        RmvAnimationAgent(actor);
        RmvPrimitiveAnimAgent(actor);
    }

    private void RmvAnimationAgent(ulong actor)
    {
        var agent = rilbucket.world.GetAgent<AnimationAgent>(actor);
        if (null == agent) return;
        rilbucket.world.RmvAgent(agent);
    }

    private void RmvPrimitiveAnimAgent(ulong actor)
    {
        var agent = rilbucket.world.GetAgent<PrimitiveAnimAgent>(actor);
        if (null == agent) return;
        rilbucket.world.RmvAgent(agent);
    }
}
