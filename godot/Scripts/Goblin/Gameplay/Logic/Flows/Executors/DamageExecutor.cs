using Goblin.Gameplay.Logic.BehaviorInfos.Flows;
using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using Goblin.Gameplay.Logic.Flows.Executors.Instructs;

namespace Goblin.Gameplay.Logic.Flows.Executors;

/// <summary>
/// 伤害结算执行器
/// </summary>
public class DamageExecutor : Executor<DamageData>
{
    protected override void OnEnter((uint pipelineid, uint index) identity, DamageData data, FlowInfo flowinfo, ulong target)
    {
        base.OnEnter(identity, data, flowinfo, target);
        Apply(data, flowinfo, target);
    }

    protected override void OnExecute((uint pipelineid, uint index) identity, DamageData data, FlowInfo flowinfo, ulong target)
    {
        base.OnExecute(identity, data, flowinfo, target);
        Apply(data, flowinfo, target);
    }

    private void Apply(DamageData data, FlowInfo flowinfo, ulong target)
    {
        var from = stage.flow.SeekETTarget(flowinfo, FLOW_DEFINE.ET_CASTER);
        var damage = stage.attrb.ChargeDamage(from, data.strength * stage.cfg.int2fp);
        stage.attrb.ToDamage(from, target, damage);
    }
}
