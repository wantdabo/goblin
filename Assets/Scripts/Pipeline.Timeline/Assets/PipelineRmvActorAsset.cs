using System.ComponentModel;
using Goblin.Gameplay.Logic.Flows.Executors.Instructs;
using Pipeline.Timeline.Assets.Common;

namespace Pipeline.Timeline.Assets
{
    [DisplayName("移除 Actor 指令")]
    public class PipelineRmvActorAsset: PipelineAsset<PipelineRmvActorAsset.PipelineRmvActorBehavior, RmvActorData>
    {
        public class PipelineRmvActorBehavior : PipelineBehavior<RmvActorData>
        {
        }
    }
}