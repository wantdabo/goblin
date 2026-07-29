using System;
using System.Collections.Generic;
using Goblin.Common;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Projection.Core;

namespace Goblin.Gameplay.Logic.Behaviors;

/// <summary>
/// 投影收集器 — 仅负责脏 BehaviorInfo → ProjectorPacket 列表
/// 挂载在 SA 上走框架 Behavior 生命周期，在 OnEndTick 中遍历 projectables 索引收集本帧脏数据
/// SG 生成的属性 setter 仅写入 projectdirtymask 位标记，由本系统帧末自检消费
/// 不做裁剪、不接触 Transport，出包后由 ProjectionPipeline 接管
/// 规模适用：数百~数千 Actor 时 projectables 遍历开销 μs 级可忽略（99% mask==0 跳过）
/// </summary>
public class ProjectorSystem : Behavior
{
    /// <summary>
    /// 本帧产出的原始投影包（未裁剪）
    /// 复用 List 避免每帧 ToArray 分配，OnEndTick 开头清空并回收上帧包
    /// 外部直接消费无需调 RecyclePackets
    /// </summary>
    public List<ProjectorPacket> packets { get; private set; }

    protected override void OnAssemble()
    {
        base.OnAssemble();
        packets = new List<ProjectorPacket>();
    }

    /// <summary>
    /// 帧末自检 — 回收上帧包 → 遍历 projectables 索引检查脏标记 → 打包 ProjectorPacket 列表
    /// 在所有 Behavior OnTick 和 OnEndTick 执行完毕后触发
    /// </summary>
    protected override void OnEndTick()
    {
        base.OnEndTick();

        // 回收上帧投影包，归还对象池
        RecyclePackets();

        // 仅遍历 IProjectable 索引，跳过非投影类的 is-check 开销
        foreach (var proj in stage.cache.projectables)
        {
            var mask = proj.projectdirtymask;
            if (0 == mask) continue;
            // IProjectable 实例均为 BehaviorInfo 子类，取 actor 用于 ProjectorPacket
            var info = proj as BehaviorInfo;
            if (null == info) continue;
            if (false == info.active) continue;
            if (0 == info.actor) continue;
            var packet = ObjectCache.Ensure<ProjectorPacket>();
            packet.actor = info.actor;
            packet.behaviorinfotype = info.GetType();
            packet.fieldmask = mask;
            packet.frame = stage.frame;
            packet.latency = 0;

            // 提取投影字段值（SG 为含 [Projector] 的类生成 IProjectable 实现）
            packet.values = proj.TakeProjectValues(mask);

            // 收集集合差量
            CollectContainerDiffs(info, mask, packet);

            // 消费后清零脏标记
            proj.projectdirtymask = 0;

            packets.Add(packet);
        }
    }

    /// <summary>
    /// 回收本帧投影包（OnEndTick 开头自动调用，外部无需关心）
    /// </summary>
    public void RecyclePackets()
    {
        if (null == packets) return;
        foreach (var packet in packets)
        {
            packet.Reset();
            ObjectCache.Set(packet);
        }
        packets.Clear();
    }

    /// <summary>
    /// 收集 GBL 容器的差量数据
    /// 通过反射检查 mask 对应的字段是否为 TGBLDict/TGBLList，是则调 CollectDiff
    /// </summary>
    private void CollectContainerDiffs(BehaviorInfo info, ulong mask, ProjectorPacket packet)
    {
        // Phase 1 暂不实现集合差量收集
        // 等 SG 生成 CollectContainerDiffs 方法后再接入
    }
}
