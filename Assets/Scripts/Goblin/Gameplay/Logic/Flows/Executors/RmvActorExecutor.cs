using Goblin.Gameplay.Logic.BehaviorInfos.Flows;
using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using Goblin.Gameplay.Logic.Flows.Executors.Instructs;

namespace Goblin.Gameplay.Logic.Flows.Executors
{
    /// <summary>
    /// 移除 Actor 执行器
    /// </summary>
    public class RmvActorExecutor : Executor<RmvActorData>
    {
        protected override void OnEnter((uint pipelineid, uint index) identity, RmvActorData data, FlowInfo flowinfo, ulong target)
        {
            base.OnEnter(identity, data, flowinfo, target);
            if (target != flowinfo.actor)
            {
                stage.RmvActor(target);
            }
            else
            {
                stage.flow.EndPipeline(target);
            }
        }
    }
}