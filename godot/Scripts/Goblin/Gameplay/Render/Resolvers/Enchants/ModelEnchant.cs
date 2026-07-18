using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Render.Agents;
using Goblin.Gameplay.Render.Resolvers.Common;

namespace Goblin.Gameplay.Render.Resolvers.Enchants;

/// <summary>
/// 模型代理的赋能
/// 按 ModelInfo.Type 路由到 ModelAgent(glb) 或 PrimitiveMeshAgent(primitive)
/// </summary>
public class ModelEnchant : AgentEnchant<RIL_FACADE_MODEL>
{
    protected override void OnRIL(RIL_FACADE_MODEL ril)
    {
        // 如果没有模型定义, 则回收全部模型代理
        if (ril.model <= 0) { RecycleAgent(ril.actor); return; }
        var modelinfo = engine.cfg.location.ModelInfos.GetOrDefault(ril.model);
        if (null == modelinfo) { RecycleAgent(ril.actor); return; }

        if ("primitive" == modelinfo.Type)
        {
            RmvModelAgent(ril.actor);
            rilbucket.world.EnsureAgent<PrimitiveMeshAgent>(ril.actor);
        }
        else
        {
            RmvPrimitiveAgent(ril.actor);
            rilbucket.world.EnsureAgent<ModelAgent>(ril.actor);
        }
    }

    protected override void OnLossRIL(RIL_LOSS ril)
    {
        base.OnLossRIL(ril);
        RecycleAgent(ril.actor);
    }

    private void RecycleAgent(ulong actor)
    {
        RmvModelAgent(actor);
        RmvPrimitiveAgent(actor);
    }

    private void RmvModelAgent(ulong actor)
    {
        var agent = rilbucket.world.GetAgent<ModelAgent>(actor);
        if (null == agent) return;
        rilbucket.world.RmvAgent(agent);
    }

    private void RmvPrimitiveAgent(ulong actor)
    {
        var agent = rilbucket.world.GetAgent<PrimitiveMeshAgent>(actor);
        if (null == agent) return;
        rilbucket.world.RmvAgent(agent);
    }
}
