namespace Goblin.SkillPipelineEditor
{
    [Name("BUFF 印下事件")]
    [Color(136/255f, 233/255f, 145/255f)]
    [Attachable(typeof(EditorEventTrack))]
    public class EditorBuffStampEventClip : EditorEventClip
    {
        [MenuName("BUFF ID")]
        public int buffid;
        [MenuName("BUFF 层数")]
        public int layer;
        [MenuName("自身生效")]
        public bool self;
        [MenuName("击中生效")]
        public bool hitstamp;
    }
}
