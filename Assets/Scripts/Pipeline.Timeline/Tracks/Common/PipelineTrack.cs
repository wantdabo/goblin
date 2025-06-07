using Pipeline.Timeline.Assets.Common;
using UnityEngine.Timeline;

namespace Pipeline.Timeline.Tracks.Common
{
    /// <summary>
    /// 管线轨道基类
    /// </summary>
    [HideInMenu]
    public class PipelineTrack : TrackAsset
    {
        protected override void OnCreateClip(TimelineClip clip)
        {
            base.OnCreateClip(clip);
        }
    }
}