using System.ComponentModel;
using Goblin.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Instructs;
using Goblin.Gameplay.Render.Common.Extensions;
using Goblin.RendererFeatures;
using Kowtow.Math;
using Pipeline.Timeline.Assets.Common;
using UnityEngine;
using UnityEngine.Playables;

namespace Pipeline.Timeline.Assets
{
    [DisplayName("碰撞检测指令")]
    public class PipelineCollisionAsset : PipelineAsset<PipelineCollisionAsset.PipelineCollisionBehavior, CollisionData>
    {
        public class PipelineCollisionBehavior : PipelineBehavior<CollisionData>
        {
            protected override void OnExecute(Playable playable, FrameData info)
            {
                base.OnExecute(playable, info);
                var position = Vector3.zero;
                var rotation = Quaternion.identity;
                if (null != PipelineWorkSpace.worker.modelgo)
                {
                    position = PipelineWorkSpace.worker.modelgo.transform.position;
                    rotation = PipelineWorkSpace.worker.modelgo.transform.rotation;
                }

                var center = position + rotation * data.offset.ToVector3();
                switch (data.overlaptype)
                {
                    case COLLISION_DEFINE.COLLISION_RAY:
                        DrawPhysRendererFeature.DrawPhysPass.DrawRay(center, position + rotation * data.raydire.ToVector3().normalized, data.raydis * Config.Int2Float, Color.yellow);
                        break;
                    case COLLISION_DEFINE.COLLISION_LINE:
                        var dire = (position + rotation * data.lineep.ToVector3()) - center;
                        DrawPhysRendererFeature.DrawPhysPass.DrawRay(center, dire.normalized, dire.magnitude, Color.yellow);
                        break;
                    case COLLISION_DEFINE.COLLISION_BOX:
                        var boxsize = data.boxsize.ToVector3();
                        DrawPhysRendererFeature.DrawPhysPass.DrawCube(center, Quaternion.identity, boxsize, Color.yellow);
                        break;
                    case COLLISION_DEFINE.COLLISION_SPHERE:
                        var radius = data.sphereradius * Config.Int2Float;
                        DrawPhysRendererFeature.DrawPhysPass.DrawSphere(center, radius, Color.yellow);
                        break;
                }
            }
        }
    }
}