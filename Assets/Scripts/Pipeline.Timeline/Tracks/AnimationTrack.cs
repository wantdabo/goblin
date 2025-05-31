using System.ComponentModel;
using Pipeline.Timeline.Assets;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Pipeline.Timeline.Tracks
{
    [DisplayName("管线/动画轨道")]
    [TrackClipType(typeof(AnimationAsset))]
    public class AnimationTrack : TrackAsset
    {
    }
}