using Goblin.Gameplay.Common.Defines;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Goblin.SkillPipelineEditor
{
    [Name("打断事件")]
    [Color(1f, 0f, 0f)]
    [Attachable(typeof(EditorEventTrack))]
    public class EditorSkillBreakEventClip : EditorEventClip
    {
        [MenuName("摇杆操作")]
        public bool joystick = false;
        [MenuName("受到伤害")]
        public bool recvhurt = false;
        [MenuName("受到控制")]
        public bool recvcontrol = false;
        [MenuName("技能释放")]
        public bool skillcast = false;
    }
}
