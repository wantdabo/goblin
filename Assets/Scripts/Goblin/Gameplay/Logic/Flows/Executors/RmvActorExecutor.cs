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
        protected override void OnEnter((uint pipelineid, uint index) identity, RmvActorData data, FlowInfo flowinfo)
        {
            base.OnEnter(identity, data, flowinfo);
            switch (data.target)
            {
                case RMV_ACTOR_DEFINE.SELF:
                    stage.flow.EndPipeline(flowinfo);
                    break;
                case RMV_ACTOR_DEFINE.OWNER:
                    stage.RmvActor(flowinfo.owner);
                    break;
            }
        }
    }
}