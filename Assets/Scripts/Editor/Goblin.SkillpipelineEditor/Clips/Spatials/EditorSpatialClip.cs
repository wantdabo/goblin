using Goblin.Gameplay.Common.Defines;
using UnityEngine;
using UnityEngine.Serialization;

namespace Goblin.SkillPipelineEditor
{ 
    [Name("变化")]
    [Color(0.0f, 1f, 1f)]
    [Attachable(typeof(EditorSpatialTrack))]
    public class EditorSpatialClip : ActionClip
    {
        [SerializeField][HideInInspector] private float length = 1f / GameDef.SP_DATA_FRAME;

        [MenuName("平移")]
        [SerializeField]
        public Vector3 position = Vector3.zero;

        [MenuName("缩放")]
        [SerializeField]
        [Range(-10, 10)]
        public float scale = 0f;

        public override float Length
        {
            get => Prefs.snapInterval;
            set => length = Prefs.snapInterval;
        }

        public override bool isValid => true;

        public EditorEffectTrack track => (EditorEffectTrack)parent;
    }
}
