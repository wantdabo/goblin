using System.ComponentModel;
using Pipeline.Timeline.Assets;
using Pipeline.Timeline.Tracks.Common;
using UnityEngine.Timeline;

namespace Pipeline.Timeline.Tracks
{
    [DisplayName("管线/POSITION 变化轨道")]
    [TrackClipType(typeof(PipelineSpatialPositionAsset))]
    public class PipelineSpatialPositionTrack : PipelineTrack
    {
        
    }
}