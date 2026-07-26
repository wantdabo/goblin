using System;
using System.Collections.Generic;
using Goblin.Common;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Projection.Core;

namespace Goblin.Gameplay.Logic.Behaviors.Sa;

/// <summary>
/// 投影收集器 — 仅负责脏 BehaviorInfo → ProjectorPacket[]
/// 挂载在 SA 上走框架 Behavior 生命周期，在 OnEndTick 中自检遍历所有 BehaviorInfo 收集本帧脏数据
/// SG 生成的属性 setter 仅写入 projectdirtymask 位标记，由本系统帧末自检消费
/// 不做裁剪、不接触 Transport，出包后由 ProjectionPipeline 接管
/// 规模适用：数百~数千 Actor 时自检遍历开销 μs 级可忽略（99% mask==0 跳过）；万级 MMO 需引入空间索引裁剪
/// </summary>
public class ProjectorSystem : Behavior
{
    /// <summary>
    /// 本帧产出的原始投影包（未裁剪）
    /// OnEndTick 开头自管理回收上帧包，外部直接消费无需调 RecyclePackets
    /// </summary>
    public ProjectorPacket[] packets { get; private set; }

    protected override void OnAssemble()
    {
        base.OnAssemble();
        packets = Array.Empty<ProjectorPacket>();
    }

    /// <summary>
    /// 帧末自检 — 回收上帧包 → 遍历 behaviorinfodict 检查脏标记 → 打包 ProjectorPacket[]
    /// 在所有 Behavior OnTick 和 OnEndTick 执行完毕后触发
    /// </summary>
    protected override void OnEndTick()
    {
        base.OnEndTick();

        // 回收上帧投影包，归还对象池
        RecyclePackets();

        List<ProjectorPacket> list = null;

        // 自检所有 BehaviorInfo 的脏标记
        foreach (var (actorId, actordict) in stage.cache.behaviorinfodict)
        {
            foreach (var info in actordict.Values)
            {
                if (false == info.active) continue;
                // 仅含 [Projector] 字段的类实现 IProjectable
                if (info is not IProjectable proj) continue;

                var mask = proj.projectdirtymask;
                if (0 == mask) continue;

                // 懒初始化，无脏数据时零分配
                if (null == list) list = new List<ProjectorPacket>();

                var packet = ObjectCache.Ensure<ProjectorPacket>();
                // 从外层 dict key 取 actorId，不依赖 info.actor（可能因对象池复用等原因为 0）
                packet.actor = actorId;
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

                list.Add(packet);
            }
        }

        // 无脏数据时零分配
        if (null == list)
        {
            packets = Array.Empty<ProjectorPacket>();
            return;
        }

        packets = list.ToArray();
        // 用普通 List.Clear() 不触发 IGBL.Reset()，元素由下一帧 RecyclePackets 回收
        list.Clear();
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
        packets = Array.Empty<ProjectorPacket>();
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
