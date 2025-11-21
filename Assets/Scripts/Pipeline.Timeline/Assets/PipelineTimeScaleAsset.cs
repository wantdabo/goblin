using System.ComponentModel;
using Goblin.Gameplay.Logic.Flows.Executors.Instructs;
using Pipeline.Timeline.Assets.Common;

namespace Pipeline.Timeline.Assets
{
    [DisplayName("时间缩放指令")]
    public class PipelineTimeScaleAsset : PipelineAsset<PipelineTimeScaleAsset.PipelineTimeScaleBehavior, TimeScaleData>
    {
        public class PipelineTimeScaleBehavior : PipelineBehavior<TimeScaleData>
        {
        }
    }
}