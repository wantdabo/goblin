using System.ComponentModel;
using Goblin.Gameplay.Logic.Flows.Executors.Instructs;
using Pipeline.Timeline.Assets.Common;

namespace Pipeline.Timeline.Assets
{
    [DisplayName("碰撞检测指令")]
    public class PipelineCollisionAsset : PipelineAsset<PipelineCollisionAsset.PipelineCollisionBehavior, CollisionData>
    {
        public class PipelineCollisionBehavior : PipelineBehavior<CollisionData>
        {
        }
    }
}