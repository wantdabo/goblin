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
        // 子弹管线：owner 是子弹 Actor，伤害在发射时已预算，直接取用
        if (stage.SeekBehaviorInfo(flowinfo.owner, out BulletInfo bulletinfo))
        {
            stage.attrc.ToDamage(bulletinfo.owner, target, bulletinfo.damage);
            return;
        }

        var damage = stage.attrc.ChargeDamage(flowinfo.owner, data.strength * stage.cfg.int2fp);
        stage.attrc.ToDamage(flowinfo.owner, target, damage);
    }
}
