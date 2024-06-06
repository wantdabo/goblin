#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Goblin.SkillPipelineEditor
{
    [Name("声音")]
    [Color(1f, 0.63f, 0f)]
    [Attachable(typeof(EditorAudioTrack))]
    public class EditorAudioClip : ActionClip, ISubClipContainable
    {
        [SerializeField] [HideInInspector] private float length = 1f;

        [MenuName("播放音频")] [SelectObjectPath(typeof(UnityEngine.AudioClip))]
        public string resPath = "";

        private UnityEngine.AudioClip _audioClip;

        public UnityEngine.AudioClip audioClip
        {
            get
            {
                if (string.IsNullOrEmpty(resPath))
                {
                    _audioClip = null;
                    return null;
                }

                if (_audioClip == null)
                {
#if UNITY_EDITOR
                    _audioClip = AssetDatabase.LoadAssetAtPath<UnityEngine.AudioClip>(resPath);
#endif
                }

                return _audioClip;
            }
        }

        [Range(0f, 1f)] [MenuName("音量")] public float volume = 1;
        [MenuName("偏移量")] public float clipOffset;


        public override float Length
        {
            get => length;
            set => length = value;
        }

        float ISubClipContainable.SubClipOffset
        {
            get => clipOffset;
            set => clipOffset = value;
        }

        float ISubClipContainable.SubClipLength => audioClip != null ? audioClip.length : 0;

        float ISubClipContainable.SubClipSpeed => 1;

        public override bool isValid => audioClip != null;

        public override string info => isValid ? audioClip.name : base.info;

        public EditorAudioTrack track => (EditorAudioTrack)parent;

#if UNITY_EDITOR
        protected override void OnClipGUI(Rect rect)
        {
            DrawTools.DrawLoopedAudioTexture(rect, audioClip, Length, clipOffset);
        }
#endif
    }
}