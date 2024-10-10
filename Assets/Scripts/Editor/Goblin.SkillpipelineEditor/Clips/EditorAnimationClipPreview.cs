using Animancer;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine;

namespace Goblin.SkillPipelineEditor
{
    /// <summary>
    /// 动画预览
    /// </summary>
    [CustomPreview(typeof(EditorAnimationClip))]
    public class EditorAnimationClipPreview : PreviewBase<EditorAnimationClip>
    {
        private AnimancerComponent animancer;
        private AnimationClip animationClip;

        public override void Enter()
        {
            var model = clip.Parent.Parent.Parent.cloneModel;
            if (model != null)
            {
                animancer = model.GetComponentInChildren<AnimancerComponent>();
            }

            if (animancer != null)
            {
                var animationClipName = string.Empty;
                if (clip.animationClip != null)
                {
                    animationClipName = clip.animationClip.name;
                }

                var clips = new List<AnimationClip>();
                animancer.GetAnimationClips(clips);
                animationClip = clips.FirstOrDefault(c => c.name == animationClipName);
            }
        }

        public override void Update(float time, float previousTime)
        {
            if (Application.isPlaying) return;
            if (time > clip.GetLength()) return;

            if (animancer != null && animationClip != null)
            {
                animationClip.SampleAnimation(animancer.gameObject, time);
            }
        }
    }
}