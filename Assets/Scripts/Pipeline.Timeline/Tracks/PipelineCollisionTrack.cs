using System.ComponentModel;
using Pipeline.Timeline.Assets;
using Pipeline.Timeline.Tracks.Common;
using UnityEngine.Timeline;

namespace Pipeline.Timeline.Tracks
{
    [DisplayName("管线/碰撞检测轨道")]
    [TrackClipType(typeof(PipelineCollisionAsset))]
    public class PipelineCollisionTrack : PipelineTrack
    {
        
    }
}