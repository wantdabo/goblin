using System.Linq;
using Goblin.Gameplay.Logic.Common.Defines;
using UnityEngine.Serialization;

namespace Goblin.SkillPipelineEditor
{
    [Name("BUFF 触发事件")]
    [Color(255/255f, 138/255f, 37/255f)]
    [Attachable(typeof(EditorEventTrack))]
    public class EditorBuffTriggerEventClip : EditorEventClip
    {
        [MenuName("BUFF")]
        [IntPopup(typeof(BuffIntPopupData))]
        public int buffid;
        
        [MenuName("自身生效")]
        [IntPopup(typeof(BuffSelfActivePopupData))]
        public int triggerself;
        
        [MenuName("目标生效")]
        [IntPopup(typeof(BuffTargetActivePopupData))]
        public int triggertarget;

        public override float Length { get => GAME_DEFINE.SP_DATA_TICK; }
    }
}