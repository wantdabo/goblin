using UnityEditor;
using UnityEngine;

namespace Goblin.SkillPipelineEditor
{
    [Name("动画")]
    [Color(0.48f, 0.71f, 0.84f)]
    [Attachable(typeof(EditorAnimationTrack))]
    public class EditorAnimationClip : ActionClip, ISubClipContainable
    {
        [SerializeField][HideInInspector] private float length = 1f;

        [MenuName("播放动画")]
        [SelectObjectPath(typeof(AnimationClip))]
        public string resPath = "";

        public AnimationClip animationClip
        {
            get
            {
                return AssetDatabase.LoadAssetAtPath<AnimationClip>(resPath);
            }
        }

        public override float Length
        {
            get => length;
            set => length = value;
        }

        float ISubClipContainable.SubClipOffset
        {
            get => 0;
            set { }
        }

        float ISubClipContainable.SubClipLength => animationClip != null ? animationClip.length : 0;

        float ISubClipContainable.SubClipSpeed => 1;

        public override bool isValid => animationClip != null;

        public override string info => isValid ? animationClip.name : base.info;

        public EditorAnimationTrack track => (EditorAnimationTrack)parent;
    }
}