using System.ComponentModel;
using Goblin.PipelineEditor.PipelineAssets;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Goblin.PipelineEditor.PipelineTracks
{
    [DisplayName("管线/动画轨道")]
    [TrackClipType(typeof(AnimationAsset))]
    public class AnimationTrack : TrackAsset, IPlayableAsset, IPropertyPreview
    {
        protected override Playable CreatePlayable(PlayableGraph graph, GameObject gameObject, TimelineClip clip)
        {
            return base.CreatePlayable(graph, gameObject, clip);
        }
    }
}