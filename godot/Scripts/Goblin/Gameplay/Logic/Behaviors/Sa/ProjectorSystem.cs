using System;
using Goblin.Common;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Projection;

namespace Goblin.Gameplay.Logic.Behaviors.Sa;

/// <summary>
/// 投影收集器 — 仅负责脏 BehaviorInfo → ProjectorPacket[]
/// 挂载在 SA 上走框架 Behavior 生命周期，在 OnEndTick 中自检遍历所有 BehaviorInfo 收集本帧脏数据
/// SG 生成的属性 setter 仅写入 projectdirtymask 位标记，由本系统帧末自检消费
/// 不做裁剪、不接触 Transport，出包后由 ProjectionPipeline 接管
/// 规模适用：数百~数千 Actor 时自检遍历开销 μs 级可忽略（99% mask==0 跳过）；万级 MMO 需引入空间索引裁剪
/// Phase 4：添加快照回滚支持
/// </summary>
public class ProjectorSystem : Behavior
{
    /// <summary>
    /// 本帧产出的原始投影包（未裁剪）
    /// OnEndTick 开头自管理回收上帧包，外部直接消费无需调 RecyclePackets
    /// </summary>
    public ProjectorPacket[] packets { get; private set; }

    // ============================================================
    // Phase 4：快照回滚
    // ============================================================

    /// <summary>
    /// 快照环形缓冲区大小
    /// </summary>
    private const int SNAPSHOT_CAPACITY = 32;
    /// <summary>
    /// 快照数组，按写入索引循环覆盖
    /// </summary>
    private readonly ProjectorSnapshot?[] snapshots = new ProjectorSnapshot?[SNAPSHOT_CAPACITY];
    /// <summary>
    /// 快照写入索引
    /// </summary>
    private int snapshotIndex = 0;
    /// <summary>
    /// 最早可回滚帧号（0 表示无快照）
    /// </summary>
    public uint earliestSnapshotFrame { get; private set; }
    /// <summary>
    /// 最新快照帧号
    /// </summary>
    public uint latestSnapshotFrame { get; private set; }

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

        GBLList<ProjectorPacket> list = null;

        // 自检所有 BehaviorInfo 的脏标记
        foreach (var actordict in stage.cache.behaviorinfodict.Values)
        {
            foreach (var info in actordict.Values)
            {
                if (false == info.active) continue;
                // 仅含 [Projector] 字段的类实现 IProjectable
                if (info is not IProjectable proj) continue;

                var mask = proj.projectdirtymask;
                if (0 == mask) continue;

                // 懒初始化，无脏数据时零分配
                if (null == list) list = ObjectCache.Ensure<GBLList<ProjectorPacket>>();

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
        list.Clear();
        ObjectCache.Set(list);

        // Phase 4：产出投影包后拍摄快照
        TakeSnapshot();
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

    // ============================================================
    // Phase 4：快照回滚
    // ============================================================

    /// <summary>
    /// 拍摄当前帧快照，保存所有 IProjectable BehaviorInfo 的投影字段值
    /// OnEndTick 末尾自动调用
    /// </summary>
    public void TakeSnapshot()
    {
        var current = stage.frame;
        var snapshot = ObjectCache.Ensure<ProjectorSnapshot>();
        snapshot.frame = current;

        var dict = ObjectCache.Ensure<GBLDict<(ulong actor, Type type), object[]>>();
        foreach (var actordict in stage.cache.behaviorinfodict.Values)
        {
            foreach (var info in actordict.Values)
            {
                if (false == info.active) continue;
                if (info is not IProjectable proj) continue;
                var key = (info.actor, info.GetType());
                dict[key] = proj.TakeProjectValues(ulong.MaxValue);
            }
        }
        snapshot.data = dict;

        // 覆盖写入环形缓冲区
        var old = snapshots[snapshotIndex];
        old?.Recycle();
        snapshots[snapshotIndex] = snapshot;
        snapshotIndex = (snapshotIndex + 1) % SNAPSHOT_CAPACITY;

        // 更新边界
        if (0 == earliestSnapshotFrame || current < earliestSnapshotFrame)
            earliestSnapshotFrame = current;
        latestSnapshotFrame = current;
    }

    /// <summary>
    /// 回滚到指定帧，恢复所有 IProjectable BehaviorInfo 值
    /// 不回滚不存在的 Actor/BehaviorInfo（已在后续帧被移除的跳过）
    /// 新 Actor（快照后创建）不处理，由 Stage.Restore 统一管理
    /// </summary>
    /// <param name="frame">目标回滚帧号</param>
    /// <returns>成功恢复</returns>
    public bool FlashRestore(uint frame)
    {
        if (false == TryFindSnapshot(frame, out var snapshot) || null == snapshot) return false;

        // 清除目标帧之后的快照
        ClearSnapshotsAfter(frame);

        // 恢复所有 BehaviorInfo 的投影字段值
        foreach (var kv in snapshot.data!)
        {
            var (actor, type) = kv.Key;
            // 从缓存直接查找 BehaviorInfo（跳过已移除的 Actor）
            if (false == stage.cache.behaviorinfodict.TryGetValue(actor, out var infodict)) continue;
            if (false == infodict.TryGetValue(type, out var info)) continue;
            if (false == info.active) continue;
            if (info is not IProjectable proj) continue;

            // 直接设置 backing field 值，不触发脏标记
            proj.SetProjectValues(kv.Value);

            // 全量标记脏：回滚后需重新同步到 RenderWorld
            proj.MarkAllDirty();
        }

        return true;
    }

    /// <summary>
    /// 环形缓冲区查找指定帧快照
    /// </summary>
    private bool TryFindSnapshot(uint frame, out ProjectorSnapshot? snapshot)
    {
        for (int i = 0; i < SNAPSHOT_CAPACITY; i++)
        {
            var s = snapshots[i];
            if (null != s && s.frame == frame)
            {
                snapshot = s;
                return true;
            }
        }
        snapshot = null;
        return false;
    }

    /// <summary>
    /// 清除指定帧之后的快照
    /// </summary>
    private void ClearSnapshotsAfter(uint frame)
    {
        for (int i = 0; i < SNAPSHOT_CAPACITY; i++)
        {
            var s = snapshots[i];
            if (null != s && (long)s.frame > (long)frame)
            {
                s.Recycle();
                snapshots[i] = null;
            }
        }
    }
}

/// <summary>
/// 投影快照 — 存储一帧所有 IProjectable BehaviorInfo 的字段值
/// </summary>
public sealed class ProjectorSnapshot : IGBL
{
    /// <summary>
    /// 快照帧号
    /// </summary>
    public uint frame { get; set; }
    /// <summary>
    /// (actor, BehaviorInfoType) → 投影字段值数组
    /// </summary>
    public GBLDict<(ulong actor, Type type), object[]>? data { get; set; }

    public void Reset()
    {
        // 回收 object[] 数组
        if (null != data)
        {
            foreach (var arr in data.Values)
            {
                arr.SetValue(null, 0);
            }
            data.Clear();
            ObjectCache.Set(data);
            data = null;
        }
        frame = 0;
    }

    public IGBL Clone()
    {
        var c = ObjectCache.Ensure<ProjectorSnapshot>();
        c.frame = frame;
        if (null != data)
        {
            c.data = ObjectCache.Ensure<GBLDict<(ulong actor, Type type), object[]>>();
            foreach (var kv in data)
            {
                var copy = new object[kv.Value.Length];
                Array.Copy(kv.Value, copy, kv.Value.Length);
                c.data[kv.Key] = copy;
            }
        }
        return c;
    }

    /// <summary>
    /// 回收快照到对象池
    /// </summary>
    public void Recycle()
    {
        Reset();
        ObjectCache.Set(this);
    }
}
