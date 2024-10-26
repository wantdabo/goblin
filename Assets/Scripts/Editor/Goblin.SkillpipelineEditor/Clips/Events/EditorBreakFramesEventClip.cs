using UnityEngine.Serialization;

namespace Goblin.SkillPipelineEditor
{
    [Name("跳帧事件")]
    [Color(0.8f, 0f, 1f)]
    [Attachable(typeof(EditorEventTrack))]
    public class EditorBreakFramesEventClip : EditorEventClip
    {
        [MenuName("自身跳帧")]
        public int selfbreakframes = 0;
        [MenuName("目标跳帧")]
        public int targetbreakframes = 0;
    }
}
