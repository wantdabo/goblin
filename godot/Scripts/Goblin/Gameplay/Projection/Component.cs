namespace Goblin.Gameplay.Projection;

/// <summary>
/// 表现层数据组件 — BehaviorInfo 的纯数据投影
/// 每个 Component 对应一个 Logic 层的 BehaviorInfo 类型，
/// Component 仅持有数据字段，Apply 时直接写入值，不包含任何渲染/插值/平滑逻辑。
/// </summary>
public abstract class Component
{
    /// <summary>
    /// 所属 Entity
    /// </summary>
    public Entity entity { get; internal set; }

    /// <summary>
    /// 对应的 Logic ActorID
    /// </summary>
    public ulong actor => entity.actor;

    /// <summary>
    /// 应用脏字段数据
    /// </summary>
    /// <param name="fieldmask">位掩码，每位对应一个字段</param>
    /// <param name="values">字段值数组，按 fieldmask 顺序排列</param>
    public abstract void Apply(ulong fieldmask, object[] values);

    /// <summary>
    /// 组件创建
    /// </summary>
    protected internal virtual void OnCreate() { }

    /// <summary>
    /// 组件销毁
    /// </summary>
    protected internal virtual void OnDestroy() { }
}
