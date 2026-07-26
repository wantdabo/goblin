using System.Collections.Generic;
using Goblin.Common;
using Goblin.Gameplay.Projection.Core;

namespace Goblin.Gameplay.Projection.Rules;

/// <summary>
/// 频率裁剪规则 — 每个字段独立推送间隔
/// Phase 1 目前过于激进，此规则后续与 NetworkTransport 配合使用
/// </summary>
public partial class FrequencyRule : IProjectionRule, IGBL
{
    /// <summary>
    /// 字段推送间隔表：键为 (BehaviorInfoType, fieldIndex)，值为间隔帧数
    /// </summary>
    private Dictionary<(System.Type, int), long> intervaltable { get; set; }

    /// <summary>
    /// 上次推送帧号表
    /// </summary>
    private Dictionary<(ulong, System.Type, int), long> lastpushtable { get; set; }

    public FrequencyRule()
    {
        intervaltable = new Dictionary<(System.Type, int), long>();
        lastpushtable = new Dictionary<(ulong, System.Type, int), long>();
    }

    /// <summary>
    /// 注册字段推送间隔
    /// </summary>
    /// <param name="behaviorInfoType">BehaviorInfo 类型</param>
    /// <param name="fieldIndex">字段 index</param>
    /// <param name="intervalFrames">推送间隔帧数</param>
    public void Add(System.Type behaviorInfoType, int fieldIndex, long intervalFrames)
    {
        intervaltable[(behaviorInfoType, fieldIndex)] = intervalFrames;
    }

    /// <summary>
    /// 裁剪：按帧间隔 mask 掉不该推送的字段
    /// </summary>
    public ulong Filter(ProjectorPacket packet, Observer observer, ulong currentmask)
    {
        if (0 == currentmask) return 0;

        // behaviorinfotype 可为 null，跳过未设置类型的包
        if (null == packet.behaviorinfotype) return currentmask;

        var result = currentmask;
        for (var i = 0; i < 64; i++)
        {
            var bit = 1ul << i;
            if (0ul == (currentmask & bit)) continue;

            var key = (packet.behaviorinfotype, i);
            if (false == intervaltable.TryGetValue(key, out var interval)) continue;

            var stateKey = (packet.actor, packet.behaviorinfotype!, i);
            if (lastpushtable.TryGetValue(stateKey, out var lastFrame))
            {
                // 仅帧号递增时做间隔检测，回滚帧（frame <= lastFrame）始终放行
                if (packet.frame > lastFrame && packet.frame - lastFrame < interval)
                {
                    // 抑制本次推送，不更新 lastpushtable
                    result &= ~bit;
                    continue;
                }
            }

            // 仅在未抑制时记录推送帧号
            lastpushtable[stateKey] = packet.frame;
        }

        return result;
    }

    /// <summary>
    /// 清理过期帧记录
    /// </summary>
    public void Cleanup(long minFrame)
    {
        // 从对象池取 List 收集过期键，避免每帧 new
        var stale = ObjectPool.Ensure<List<(ulong, System.Type, int)>>("FREQUENCY_CLEANUP_LIST");
        stale.Clear();
        foreach (var kv in lastpushtable)
        {
            if (kv.Value < minFrame) stale.Add(kv.Key);
        }
        foreach (var key in stale) lastpushtable.Remove(key);
        ObjectPool.Set(stale, "FREQUENCY_CLEANUP_LIST");
    }

    /// <summary>
    /// 浅拷贝
    /// </summary>
    public IGBL Clone()
    {
        var copy = (FrequencyRule)MemberwiseClone();
        copy.intervaltable = new Dictionary<(System.Type, int), long>(intervaltable);
        copy.lastpushtable = new Dictionary<(ulong, System.Type, int), long>(lastpushtable);
        return copy;
    }

    /// <summary>
    /// 重置状态
    /// </summary>
    public void Reset()
    {
        intervaltable.Clear();
        lastpushtable.Clear();
    }
}
