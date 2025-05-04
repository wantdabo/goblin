using UnityEngine;
using UnityEngine.Serialization;

namespace Goblin.SkillPipelineEditor
{
    [Name("BUFF 印下事件")]
    [Color(136/255f, 233/255f, 145/255f)]
    [Attachable(typeof(EditorEventTrack))]
    public class EditorBuffStampEventClip : EditorEventClip
    {
        [MenuName("BUFF")]
        [IntPopup(typeof(BuffIntPopupData))]
        public int buffid;

        [MenuName("自身生效")]
        [IntPopup(typeof(BuffSelfActivePopupData))]
        public int stampself;
        
        [MenuName("目标生效")]
        [IntPopup(typeof(BuffTargetActivePopupData))]
        public int stamptarget;
        
        [MenuName("BUFF 层数")]
        public int layer;
    }
}
