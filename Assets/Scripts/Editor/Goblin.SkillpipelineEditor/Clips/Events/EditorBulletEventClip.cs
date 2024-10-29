using Goblin.Gameplay.Common.Defines;
using UnityEngine;

namespace Goblin.SkillPipelineEditor
{
    [Name("子弹事件")]
    [Color(0f, 0f, 1f)]
    [Attachable(typeof(EditorEventTrack))]
    public class EditorBulletEventClip : EditorEventClip
    {
        [MenuName("子弹 ID")]
        public int bulletid = 0;
        [MenuName("起始位置")]
        public Vector3 position = Vector3.zero;
        
        public override float Length => GameDef.SP_DATA_TICK;
    }
}
