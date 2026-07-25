using System;

namespace Goblin.Gameplay.Projection;

/// <summary>
/// 按 Observer 裁剪后的数据包 — Crop 产出，Transport 消费
/// </summary>
public class ObserverPacket
{
    /// <summary>
    /// 目标观察者
    /// </summary>
    public Observer observer { get; set; }

    /// <summary>
    /// ActorID
    /// </summary>
    public ulong actor { get; set; }

    /// <summary>
    /// BehaviorInfo 类型
    /// </summary>
    public Type behaviorinfotype { get; set; }

    /// <summary>
    /// 裁剪后的字段掩码
    /// </summary>
    public ulong fieldmask { get; set; }

    /// <summary>
    /// Logic 帧号
    /// </summary>
    public long frame { get; set; }

    /// <summary>
    /// 裁剪后的字段值数组（与 fieldmask 对齐）
    /// </summary>
    public object[] values { get; set; }
}
