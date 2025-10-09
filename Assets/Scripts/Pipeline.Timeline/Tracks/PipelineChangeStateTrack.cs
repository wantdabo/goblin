using System.ComponentModel;
using Pipeline.Timeline.Assets;
using Pipeline.Timeline.Tracks.Common;
using UnityEngine.Timeline;

namespace Pipeline.Timeline.Tracks
{
    [DisplayName("管线/变更状态轨道")]
    [TrackClipType(typeof(PipelineChangeStateAsset))]
    public class PipelineChangeStateTrack : PipelineTrack
    {
        
    }
}