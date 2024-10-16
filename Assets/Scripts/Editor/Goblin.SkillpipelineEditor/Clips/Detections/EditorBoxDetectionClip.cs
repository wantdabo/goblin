using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Goblin.SkillPipelineEditor
{
    [Name("立方体碰撞")]
    [Color(0.6f, 0.839f, 0.325f)]
    [Attachable(typeof(EditorDetectionTrack))]
    public class EditorBoxDetectionClip : EditorDetectionClip
    {
        [MenuName("尺寸")]
        [SerializeField]
        public Vector3 size = Vector3.one;
    }
}
