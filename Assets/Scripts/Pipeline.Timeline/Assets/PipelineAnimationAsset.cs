using System.ComponentModel;
using Animancer;
using Goblin.Gameplay.Logic.Flows.Executors.Instructs;
using Pipeline.Timeline.Assets.Common;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Pipeline.Timeline.Assets
{
    [DisplayName("动画指令")]
    public class PipelineAnimationAsset : PipelineAsset<PipelineAnimationAsset.PipelineAnimationBehavior, AnimationData>
    {
        public class PipelineAnimationBehavior : PipelineBehavior<AnimationData>
        {
            protected override void OnExecute(Playable playable, FrameData info)
            {
                base.OnExecute(playable, info);
                if (null == PipelineWorkSpace.worker.modelgo) return;
                var animancer = PipelineWorkSpace.worker.modelgo.GetComponent<NamedAnimancerComponent>();
                if (null == animancer) return;
                AnimationClip clip = default;
                foreach (var c in animancer.Animations)
                {
                    if (false == c.name.Equals(data.name)) continue;
                    clip = c;
                    break;
                }
                
                if (null == clip) return;
                clip.SampleAnimation(PipelineWorkSpace.worker.modelgo, (float)playable.GetTime());
            }
        }
    }
}