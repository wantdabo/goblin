using System.ComponentModel;
using Animancer;
using Goblin.Common;
using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Instructs;
using Goblin.Gameplay.Render.Common.Extensions;
using Pipeline.Timeline.Assets.Common;
using UnityEngine;
using UnityEngine.Playables;

namespace Pipeline.Timeline.Assets
{
    [DisplayName("POSITION 变化指令")]
    public class PipelineSpatialPositionAsset : PipelineAsset<PipelineSpatialPositionAsset.PipelineSpatialPositionBehavior, SpatialPositionData>
    {
        public class PipelineSpatialPositionBehavior : PipelineBehavior<SpatialPositionData>
        {
            protected override void OnPass(Playable playable, FrameData info)
            {
                base.OnPass(playable, info);
                var position = data.position.ToVector3();
                SetPositionOffset(position);
            }

            protected override void OnReverse(Playable playable, FrameData info)
            {
                base.OnReverse(playable, info);
                var position = -data.position.ToVector3();
                SetPositionOffset(position);
            }

            private void SetPositionOffset(Vector3 offset)
            {
                if (null == PipelineWorkSpace.worker.modelgo) return;

                switch (data.type)
                {
                    case SPATIAL_DEFINE.POSITION_WORLD:
                        PipelineWorkSpace.worker.modelgo.transform.position += offset;
                        break;
                    case SPATIAL_DEFINE.POSITION_SELF:
                        var rotation = PipelineWorkSpace.worker.modelgo.transform.rotation;
                        offset = rotation * offset;
                        PipelineWorkSpace.worker.modelgo.transform.position += offset;
                        break;
                }
            }
        }
    }
}