using System.ComponentModel;
using Pipeline.Timeline.Assets;
using Pipeline.Timeline.Tracks.Common;
using UnityEngine.Timeline;

namespace Pipeline.Timeline.Tracks
{
    [DisplayName("管线/移除 Actor 轨道")]
    [TrackClipType(typeof(PipelineRmvActorAsset))]
    public class PipelineRmvActorTrack : PipelineTrack
    {
        
    }
}