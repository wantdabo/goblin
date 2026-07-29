using Goblin.Gameplay.Logic.BehaviorInfos.Flows;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using Goblin.Gameplay.Logic.Flows.Executors.Instructs;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Flows.Executors;

/// <summary>
/// 状态变更执行器
/// </summary>
public class ChangeStateExecutor : Executor<ChangeStateData>
{
    protected override void OnEnter((uint pipelineid, uint index) identity, ChangeStateData data, FlowInfo flowinfo, ulong target)
    {
        base.OnEnter(identity, data, flowinfo, target);
        if (data.breakable)
        {
            stage.statemachine.Break(target);
            return;
        }

        if (data.force)
        {
            stage.statemachine.ChangeState(target, data.state);
        }
        else
        {
            stage.statemachine.TryChangeState(target, data.state);
        }

        if (data.usedelaybreak)
        {
            stage.statemachine.Break(target, data.delaybreak * FP.EN3);
        }
    }
}
