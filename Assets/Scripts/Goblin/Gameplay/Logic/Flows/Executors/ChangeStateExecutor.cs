using Goblin.Gameplay.Logic.BehaviorInfos.Flows;
using Goblin.Gameplay.Logic.Behaviors;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using Goblin.Gameplay.Logic.Flows.Executors.Instructs;

namespace Goblin.Gameplay.Logic.Flows.Executors
{
    /// <summary>
    /// 状态变更执行器
    /// </summary>
    public class ChangeStateExecutor : Executor<ChangeStateData>
    {
        protected override void OnEnter((uint pipelineid, uint index) identity, ChangeStateData data, FlowInfo flowinfo)
        {
            base.OnEnter(identity, data, flowinfo);
            if (false == stage.SeekBehavior(flowinfo.owner, out StateMachine statemachine)) return;

            if (data.breakable)
            {
                statemachine.Break();
                return;
            }
            
            if (data.force)
            {
                statemachine.ChangeState(data.state);
                return;
            }
            
            statemachine.ChangeState(data.state);
        }
    }
}