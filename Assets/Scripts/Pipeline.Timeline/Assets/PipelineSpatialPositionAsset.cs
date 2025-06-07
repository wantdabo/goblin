using System.ComponentModel;
using Animancer;
using Goblin.Common;
using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Instructs;
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
                SetPositionOffset(new Vector3(data.x, data.y, data.z) * Config.Int2Float);
            }

            protected override void OnReverse(Playable playable, FrameData info)
            {
                base.OnReverse(playable, info);
                SetPositionOffset(new Vector3(-data.x, -data.y, -data.z) * Config.Int2Float);
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
                        break;
                }
            }
        }
    }
}