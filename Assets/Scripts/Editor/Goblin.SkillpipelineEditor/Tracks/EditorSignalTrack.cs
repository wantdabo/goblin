using UnityEngine.EventSystems;

namespace Goblin.SkillPipelineEditor
{
    [Name("信号轨道")]
    [Description("这是一个信号轨道，用来发送一些如受击信号等")]
    [Attachable(typeof(Group))]
    [ShowIcon(typeof(EventSystem))]
    public class EditorSignalTrack : Track
    {
        
    }
}