namespace Goblin.Gameplay.Projection;

/// <summary>
/// 表现组件 — 纯数据容器，接收投影数据
/// Phase 1 不做表达，仅 Apply 写入字段
/// SG 为含 [Projector] 映射的子类生成 Apply override（T1.9 后续）
/// </summary>
public abstract class Component
{
    /// <summary>
    /// 所属实体
    /// </summary>
    public Entity entity { get; internal set; }
    /// <summary>
    /// ActorID
    /// </summary>
    public ulong actor => entity.actor;

    /// <summary>
    /// 按 fieldmask 将 values[] 写入组件字段
    /// values 按 mask 位从低到高排列，SG 生成的 override 按字段 index 顺序消费
    /// </summary>
    /// <param name="fieldmask">脏字段掩码</param>
    /// <param name="values">脏字段值数组</param>
    public abstract void Apply(ulong fieldmask, object[] values);

    /// <summary>
    /// 推入历史缓冲区
    /// Phase 1 直接 Apply，不缓冲（Phase 2 扩充 ring buffer 做插值）
    /// </summary>
    /// <param name="frame">Logic 帧号</param>
    /// <param name="latency">滞后帧数</param>
    /// <param name="fieldmask">脏字段掩码</param>
    /// <param name="values">脏字段值数组</param>
    internal void PushHistory(long frame, int latency, ulong fieldmask, object[] values)
    {
        // Phase 1：直接应用，不缓冲
        Apply(fieldmask, values);
    }

    /// <summary>
    /// 创建时调用（Entity.AddComp 触发）
    /// </summary>
    protected internal virtual void OnCreate() { }

    /// <summary>
    /// 销毁时调用
    /// </summary>
    protected internal virtual void OnDestroy() { }
}
