using Goblin.Gameplay.Common.Defines;
using System.Linq;

namespace Goblin.SkillPipelineEditor
{
    [Name("BUFF 触发事件")]
    [Color(28/255f, 138/255f, 37/255f)]
    [Attachable(typeof(EditorEventTrack))]
    public class EditorBuffTriggerEventClip : EditorEventClip
    {
        [MenuName("BUFF")]
        [IntPopup(typeof(BuffIntPopupData))]
        public int buffid;

        public override float Length { get => GAME_DEFINE.SP_DATA_TICK; }
    }
}