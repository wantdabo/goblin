using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Goblin.SkillPipelineEditor
{
    [Name("球体碰撞")]
    [Color(0.6f, 0.839f, 0.325f)]
    [Attachable(typeof(EditorDetectionTrack))]
    public class EditorDetectionSphereClip : EditorDetectionClip
    {
        [MenuName("半径")]
        [SerializeField]
        public float radius = 1f;
    }
}