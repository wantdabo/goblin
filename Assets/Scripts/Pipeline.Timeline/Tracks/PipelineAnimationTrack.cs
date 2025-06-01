using System.ComponentModel;
using Goblin.Gameplay.Logic.Flows.Executors.Instructs;
using Pipeline.Timeline.Assets;
using Pipeline.Timeline.Tracks.Common;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Pipeline.Timeline.Tracks
{
    [DisplayName("管线/动画轨道")]
    [TrackClipType(typeof(PipelineAnimationAsset))]
    public class PipelineAnimationTrack : PipelineTrack
    {
    }
}   