﻿#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Serialization;

namespace Goblin.SkillPipelineEditor
{
    [Name("声音")]
    [Color(1f, 0.63f, 0f)]
    [Attachable(typeof(EditorSoundTrack))]
    public class EditorSoundClip : ActionClip, ISubClipContainable
    {
        [SerializeField] [HideInInspector] private float length = 1f;

        [MenuName("播放音效")]
        [SelectObjectPath(typeof(GameObject))]
        public string res = "";

        public UnityEngine.AudioClip audioClip
        {
            get
            {
                if (string.IsNullOrEmpty(res))
                {
                    return null;
                }
                var obj = AssetDatabase.LoadAssetAtPath<GameObject>(res);
                if (null == obj) return null;
                var audioSource = obj.GetComponent<AudioSource>();
                if (null == audioSource) return null;
                
                return audioSource.clip;
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

        public EditorSoundTrack track => (EditorSoundTrack)parent;

#if UNITY_EDITOR
        protected override void OnClipGUI(Rect rect)
        {
            DrawTools.DrawLoopedAudioTexture(rect, audioClip, Length, clipOffset);
        }
#endif
    }
}