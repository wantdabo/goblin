using Goblin.SkillPipelineEditor;
using UnityEngine;

[Name("事件")]
[Color(1f, 0f, 0f)]
[Attachable(typeof(EditorEventTrack))]
public abstract class EditorEventClip : ActionClip
{
    [SerializeField][HideInInspector] private float length = 1f;
    
    public override float Length
    {
        get => length;
        set => length = value;
    }

    public override bool isValid => true;

    public EditorEventTrack track => (EditorEventTrack)parent;
}
