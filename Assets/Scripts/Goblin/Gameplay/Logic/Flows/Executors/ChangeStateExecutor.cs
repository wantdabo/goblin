using Goblin.Gameplay.Logic.BehaviorInfos.Flows;
using Goblin.Gameplay.Logic.Behaviors;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using Goblin.Gameplay.Logic.Flows.Executors.Instructs;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Flows.Executors
{
    /// <summary>
    /// 状态变更执行器
    /// </summary>
    public class ChangeStateExecutor : Executor<ChangeStateData>
    {
        protected override void OnEnter((uint pipelineid, uint index) identity, ChangeStateData data, FlowInfo flowinfo, ulong target)
        {
            base.OnEnter(identity, data, flowinfo, target);
            if (false == stage.SeekBehavior(target, out StateMachine statemachine)) return;

            if (data.breakable)
            {
                statemachine.Break();
                return;
            }

            if (data.force)
            {
                statemachine.ChangeState(data.state);
            }
            else
            {
                statemachine.TryChangeState(data.state);
            }
            
            if (data.usedelaybreak)
            {
                statemachine.Break(data.delaybreak * FP.EN3);
            }
        }
    }
}