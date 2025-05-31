using System.ComponentModel;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Pipeline.Timeline.Assets
{
    [DisplayName("动画指令")]
    public class AnimationAsset : PlayableAsset, IPlayableAsset
    {
        public AnimationClip animationClip;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return ScriptPlayable<ManualSamplePlayableBehaviour>.Create(graph, 1);
        }

        class ManualSamplePlayableBehaviour : PlayableBehaviour
        {
            private GameObject model { get; set; }
            public AnimationClip clip { get; set; }

            public override void ProcessFrame(Playable playable, FrameData info, object playerData)
            {
                if (clip == null || model == null)
                    return;

                // 手动计算时间
                double time = playable.GetTime();
                clip.SampleAnimation(model, (float)time);
            }

            public override void OnGraphStart(Playable playable)
            {
                var director = playable.GetGraph().GetResolver() as PlayableDirector;
                if (director != null) model = director.gameObject;
            }
        }
    }
}