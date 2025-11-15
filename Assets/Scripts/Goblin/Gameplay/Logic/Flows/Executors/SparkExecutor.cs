using Goblin.Gameplay.Logic.BehaviorInfos.Flows;
using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using Goblin.Gameplay.Logic.Flows.Executors.Instructs;

namespace Goblin.Gameplay.Logic.Flows.Executors
{
    /// <summary>
    /// 火花执行器
    /// </summary>
    public class SparkExecutor : Executor<SparkData>
    {
        protected override void OnEnter((uint pipelineid, uint index) identity, SparkData data, FlowInfo flowinfo)
        {
            base.OnEnter(identity, data, flowinfo);
            stage.flow.Spark(flowinfo, data.influence, data.token);
        }
    }
}