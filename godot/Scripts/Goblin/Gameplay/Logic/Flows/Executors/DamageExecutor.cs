using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.BehaviorInfos.Flows;
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
        // 如果管线归属于 Magic Actor，施法者是 Magic 的 owner
        var from = stage.SeekBehaviorInfo(flowinfo.owner, out MagicInfo magicinfo)
            ? magicinfo.owner
            : flowinfo.owner;

        var damage = stage.attrb.ChargeDamage(from, data.strength * stage.cfg.int2fp);
        stage.attrb.ToDamage(from, target, damage);
    }
}
