using UnityEngine;
using UnityEngine.Serialization;

namespace Goblin.SkillPipelineEditor
{ 
    [Name("变化")]
    [Color(0.0f, 1f, 1f)]
    [Attachable(typeof(EditorTransformTrack))]
    public class EditorTransformClip : ActionClip
    {
        [SerializeField][HideInInspector] private float length = 1f;

        [MenuName("平移")]
        [SerializeField]
        public Vector3 position = Vector3.zero;

        [FormerlySerializedAs("euler")]
        [MenuName("旋转")]
        [SerializeField]
        public Vector3 eulerAngle = Vector3.zero;

        [MenuName("缩放")]
        [SerializeField]
        [Range(-10, 10)]
        public float scale = 0f;

        public override float Length
        {
            get => length;
            set => length = value;
        }

        public override bool isValid => true;

        public EditorEffectTrack track => (EditorEffectTrack)parent;
    }
}
