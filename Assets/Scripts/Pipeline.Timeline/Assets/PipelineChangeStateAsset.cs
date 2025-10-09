using System.ComponentModel;
using Goblin.Gameplay.Logic.Flows.Executors.Instructs;
using Pipeline.Timeline.Assets.Common;

namespace Pipeline.Timeline.Assets
{
    [DisplayName("变更状态指令")]
    public class PipelineChangeStateAsset : PipelineAsset<PipelineChangeStateAsset.PipelineChangeStateBehavior, ChangeStateData>
    {
        public class PipelineChangeStateBehavior : PipelineBehavior<ChangeStateData>
        {
        }
    }
}