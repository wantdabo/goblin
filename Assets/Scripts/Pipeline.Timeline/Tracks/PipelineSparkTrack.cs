using System.ComponentModel;
using Pipeline.Timeline.Assets;
using Pipeline.Timeline.Tracks.Common;
using UnityEngine.Timeline;

namespace Pipeline.Timeline.Tracks
{
    [DisplayName("管线/火花指令轨道")]
    [TrackClipType(typeof(PipelineSparkAsset))]
    public class PipelineSparkTrack : PipelineTrack
    {
        
    }
}