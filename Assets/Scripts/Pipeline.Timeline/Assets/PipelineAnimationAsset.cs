using System.ComponentModel;
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
    public class PipelineAnimationAsset : PipelineAsset<PipelineAnimationAsset.PipelineAnimationBehavior, AnimationData>, IPlayableAsset
    {
        public class PipelineAnimationBehavior : PipelineBehavior<AnimationData>
        {
            public override void ProcessFrame(Playable playable, FrameData info, object playerData)
            {
                base.ProcessFrame(playable, info, playerData);
            }
        }
    }
}