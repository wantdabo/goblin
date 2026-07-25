using System;

namespace Goblin.Gameplay.Projection.Core;

/// <summary>
/// 裁剪后的观察者数据包 — 经 Crop 规则链裁剪后，发给特定 Observer 的数据
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
    /// 字段值数组
    /// </summary>
    public object[] values { get; set; }
}
