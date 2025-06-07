using System.ComponentModel;
using Pipeline.Timeline.Assets;
using Pipeline.Timeline.Tracks.Common;
using UnityEngine.Timeline;

namespace Pipeline.Timeline.Tracks
{
    [DisplayName("管线/释放技能轨道")]
    [TrackClipType(typeof(PipelineLaunchSkillAsset))]
    public class PipelineLaunchSkillTrack : PipelineTrack
    {
        
    }
}