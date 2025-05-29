using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Goblin.PipelineEditor.PipelineAssets
{
    public class AnimationAsset : PlayableAsset, IPlayableAsset,ITimelineClipAsset, IPropertyPreview
    {
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            throw new System.NotImplementedException();
        }

        public ClipCaps clipCaps { get; }
        public void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
            throw new System.NotImplementedException();
        }
    }
}