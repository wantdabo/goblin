namespace Goblin.Gameplay.Render.Components;

/// <summary>
/// 标记 Component 对应的 BehaviorInfo 类型，SG 据此生成 ApplyTo 方法
/// </summary>
[System.AttributeUsage(System.AttributeTargets.Class)]
public class ProjectorTargetAttribute : System.Attribute
{
    public System.Type infotype { get; }

    public ProjectorTargetAttribute(System.Type infotype)
    {
        this.infotype = infotype;
    }
}
