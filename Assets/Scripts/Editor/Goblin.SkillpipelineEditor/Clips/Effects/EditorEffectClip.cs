using Goblin.SkillPipelineEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Goblin.SkillPipelineEditor
{
    [Name("特效")]
    [Color(0.0f, 1f, 1f)]
    [Attachable(typeof(EditorEffectTrack))]
    public class EditorEffectClip : ActionClip
    {
        [SerializeField][HideInInspector] private float length = 1f;

        [FormerlySerializedAs("resPath")]
        [MenuName("特效对象")]
        [SelectObjectPath(typeof(GameObject))]
        public string res = "";
        [MenuName("平移")]
        [SerializeField]
        public Vector3 position;
        [MenuName("旋转")]
        [SerializeField]
        public Vector3 eulerAngle;
        [MenuName("缩放")]
        [SerializeField]
        [Range(0.0f, 10)]
        public float scale = 1f;
        [FormerlySerializedAs("positionBinding")]
        [MenuName("平移.目标绑定")]
        [SerializeField]
        public bool binding = false;

        public override float Length
        {
            get => length;
            set => length = value;
        }

        public override bool isValid => false == string.IsNullOrEmpty(res);

        public override string info => isValid ? res : base.info;

        public EditorEffectTrack track => (EditorEffectTrack)parent;
    }
}