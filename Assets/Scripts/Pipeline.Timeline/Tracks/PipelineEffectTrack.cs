using System.ComponentModel;
using Pipeline.Timeline.Assets;
using Pipeline.Timeline.Tracks.Common;
using UnityEngine.Timeline;

namespace Pipeline.Timeline.Tracks
{
    [DisplayName("管线/特效轨道")]
    [TrackClipType(typeof(PipelineEffectAsset))]
    public class PipelineEffectTrack : PipelineTrack
    {
        
    }
}