using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Goblin.SkillPipelineEditor
{
    public abstract class EditorDetectionClip : ActionClip
    {
        [MenuName("平移")]
        [SerializeField]
        public Vector3 position;

        [SerializeField][HideInInspector] private float length = 1f;
        public override float Length
        {
            get => length;
            set => length = Mathf.Max(value, Prefs.snapInterval);
        }
        public override bool isValid => true;

        public EditorDetectionTrack track => (EditorDetectionTrack)parent;
    }
}
