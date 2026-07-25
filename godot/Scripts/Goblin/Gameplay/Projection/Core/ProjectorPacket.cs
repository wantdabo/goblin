using System;
using Goblin.Common;
using Goblin.Gameplay.Logic.Common;

namespace Goblin.Gameplay.Projection.Core;

/// <summary>
/// 投影数据包 — ProjectorSystem Tick 产出的单条同步数据
/// </summary>
public class ProjectorPacket : IGBL
{
    /// <summary>
    /// ActorID
    /// </summary>
    public ulong actor { get; set; }

    /// <summary>
    /// BehaviorInfo 类型（Render 侧据此映射 Component）
    /// </summary>
    public Type behaviorinfotype { get; set; }

    /// <summary>
    /// 脏字段掩码，位图对应 [Projector(index)] 字段
    /// </summary>
    public ulong fieldmask { get; set; }

    /// <summary>
    /// Logic 帧号
    /// </summary>
    public long frame { get; set; }

    /// <summary>
    /// 滞后帧数（帧同步时恒 0）
    /// </summary>
    public int latency { get; set; }

    /// <summary>
    /// 字段值数组，values[i] 对应 [Projector(index: i)] 的当前值
    /// </summary>
    public object[] values { get; set; }

    /// <summary>
    /// 集合类型：新增的 key 列表
    /// </summary>
    public GBLList<uint> addedkeys { get; set; }

    /// <summary>
    /// 集合类型：移除的 key 列表
    /// </summary>
    public GBLList<uint> removedkeys { get; set; }

    /// <summary>
    /// 重置，回收前调用 — 归还内部 GBLList 并清空字段
    /// </summary>
    public void Reset()
    {
        addedkeys?.Reset();
        ObjectCache.Set(addedkeys);
        addedkeys = null;
        removedkeys?.Reset();
        ObjectCache.Set(removedkeys);
        removedkeys = null;

        actor = 0;
        behaviorinfotype = null;
        fieldmask = 0;
        frame = 0;
        latency = 0;
        values = null;
    }

    /// <summary>
    /// 浅拷贝（当前无需深度克隆场景）
    /// </summary>
    public IGBL Clone()
    {
        return (IGBL)MemberwiseClone();
    }
}
