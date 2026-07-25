namespace Goblin.Gameplay.Projection.Core;

/// <summary>
/// 可投影接口 — 含 [Projector] 注解的 BehaviorInfo 由 SG 生成实现
/// 隔离投影职责，BehaviorInfo 基类不耦合投影概念
/// ProjectorSystem 通过此接口自检脏标记并提取投影值
/// </summary>
public interface IProjectable
{
    /// <summary>
    /// 投影脏标记，位图对应 [Projector(index)] 字段
    /// SG 生成的属性 setter 自动写入
    /// </summary>
    ulong projectdirtymask { get; set; }

    /// <summary>
    /// 按脏标记掩码收集值到 object[]
    /// </summary>
    /// <param name="mask">脏字段掩码</param>
    /// <returns>脏字段值数组</returns>
    object[] TakeProjectValues(ulong mask);

    /// <summary>
    /// 从 object[] 设置 backing field 值（不触发脏标记）
    /// Phase 4 快照回滚时使用
    /// </summary>
    /// <param name="values">全量字段值数组，索引对应 [Projector(index)]</param>
    void SetProjectValues(object[] values);

    /// <summary>
    /// 标记全部投影字段为脏（新对象首帧全量同步）
    /// </summary>
    void MarkAllDirty();
}
