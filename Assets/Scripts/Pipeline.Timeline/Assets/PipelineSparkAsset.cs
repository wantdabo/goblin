using System.ComponentModel;
using Goblin.Gameplay.Logic.Flows.Executors.Instructs;
using Pipeline.Timeline.Assets.Common;

namespace Pipeline.Timeline.Assets
{
    [DisplayName("火花指令")]
    public class PipelineSparkAsset : PipelineAsset<PipelineSparkAsset.PipelineSparkBehavior, SparkData>
    {
        public class PipelineSparkBehavior : PipelineBehavior<SparkData>
        {
        }
    }
}