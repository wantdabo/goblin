using System;
using System.Collections.Generic;
using System.Numerics;
using Goblin.Common;
using Goblin.Gameplay.Projection.Core;

namespace Goblin.Gameplay.Projection.Rules;

/// <summary>
/// 裁剪规则链 — 串联多个 IProjectionRule，逐步修剪 fieldmask
/// mask == 0 时丢弃整条数据包
/// </summary>
public partial class Crop : IGBL
{
    /// <summary>
    /// 规则链
    /// </summary>
    private List<IProjectionRule> rules { get; set; }

    /// <summary>
    /// 初始化（Phase 1 默认挂 GodRule）
    /// </summary>
    public Crop()
    {
        rules = new List<IProjectionRule>();
    }

    /// <summary>
    /// 添加规则（追加到链尾）
    /// </summary>
    public void AddRule(IProjectionRule rule)
    {
        rules.Add(rule);
    }

    /// <summary>
    /// 移除规则
    /// </summary>
    public void RmvRule(IProjectionRule rule)
    {
        rules.Remove(rule);
    }

    /// <summary>
    /// 对单个数据包执行规则链裁剪
    /// </summary>
    /// <param name="packet">原始数据包</param>
    /// <param name="observer">目标观察者</param>
    /// <returns>裁剪后的 fieldmask，0 表示丢弃</returns>
    public ulong Project(ProjectorPacket packet, Observer observer)
    {
        ulong mask = packet.fieldmask;
        foreach (var rule in rules)
        {
            mask = rule.Filter(packet, observer, mask);
            if (0 == mask) return 0;
        }
        return mask;
    }

    /// <summary>
    /// 对一组数据包执行裁剪，产出 ObserverPacket 数组
    /// 复杂度 O(N×M)，大量实体 + 多观察者场景需考虑空间分区优化
    /// 单 Observer 快路径：mask 未裁剪时直接共享原 values 引用，跳过 TrimValues 分配
    /// </summary>
    /// <param name="packets">原始数据包列表</param>
    /// <param name="observers">观察者列表</param>
    /// <returns>裁剪后的 ObserverPacket 数组，mask == 0 的已过滤</returns>
    public static ObserverPacket[] Process(IReadOnlyList<ProjectorPacket> packets, List<Observer> observers)
    {
        if (0 == packets.Count || 0 == observers.Count) return Array.Empty<ObserverPacket>();

        // 单 Observer 快路径：mask 未裁剪时共享原 values 引用，零分配
        if (1 == observers.Count)
        {
            var obs = observers[0];
            var crop = obs.crop;
            // 预分配最大可能大小的数组，避免 List 中间容器 + ToArray 分配
            var resultsArray = new ObserverPacket[packets.Count];
            int ri = 0;
            foreach (var p in packets)
            {
                var mask = null != crop ? crop.Project(p, obs) : p.fieldmask;
                if (0 == mask) continue;
                var op = ObjectPool.Ensure<ObserverPacket>(ObserverPacket.POOL_KEY);
                op.observer = obs;
                op.actor = p.actor;
                op.behaviorinfotype = p.behaviorinfotype;
                op.fieldmask = mask;
                op.frame = p.frame;
                // mask == p.fieldmask 时无裁剪，直接共享原 values 引用
                // LocalTransport.ApplyPackets 同步消费，下帧 OnEndTick 前 RecyclePackets 回收
                op.values = (mask == p.fieldmask) ? p.values : TrimValues(p.values, p.fieldmask, mask);
                resultsArray[ri++] = op;
            }
            // 无过滤时直接返回，有过滤时截取
            return ri == packets.Count ? resultsArray : resultsArray[..ri];
        }
        else
        {
            // 多 Observer 路径：每个 Observer 独立 TrimValues，避免共享引用导致的数据竞争
            var maxResults = packets.Count * observers.Count;
            var resultsArray = new ObserverPacket[maxResults];
            int ri = 0;
            foreach (var p in packets)
            {
                foreach (var obs in observers)
                {
                    var crop = obs.crop;
                    var mask = null != crop ? crop.Project(p, obs) : p.fieldmask;
                    if (0 == mask) continue;
                    var trimmed = TrimValues(p.values, p.fieldmask, mask);
                    var op = ObjectPool.Ensure<ObserverPacket>(ObserverPacket.POOL_KEY);
                    op.observer = obs;
                    op.actor = p.actor;
                    op.behaviorinfotype = p.behaviorinfotype;
                    op.fieldmask = mask;
                    op.frame = p.frame;
                    op.values = trimmed;
                    resultsArray[ri++] = op;
                }
            }
            return ri == maxResults ? resultsArray : resultsArray[..ri];
        }
    }

    /// <summary>
    /// 获取规则链中所有规则（用于注入委托）
    /// </summary>
    public IEnumerable<IProjectionRule> GetRules()
    {
        return rules;
    }

    /// <summary>
    /// 清理频率规则中的过期条目
    /// </summary>
    public void CleanupFrequencyRules(long minFrame)
    {
        foreach (var rule in rules)
        {
            if (rule is FrequencyRule freq) freq.Cleanup(minFrame);
        }
    }

    /// <summary>
    /// 按裁剪后 mask 从原始 values 中提取子集
    /// 使用 BitOperations.TrailingZeroCount + PopCount 只遍历 targetMask 中已设置的位
    /// 对引用类型值做安全克隆，防止多线程下逻辑层原地修改导致数据竞争
    /// </summary>
    private static object[] TrimValues(object[] values, ulong originalMask, ulong targetMask)
    {
        if (null == values || 0 == targetMask) return Array.Empty<object>();

        int count = BitOperations.PopCount(targetMask);
        var result = new object[count];
        int ri = 0;
        var remaining = targetMask;
        while (remaining != 0)
        {
            int bit = BitOperations.TrailingZeroCount(remaining);
            remaining &= remaining - 1;
            // 该 bit 在 originalMask 中对应的 values 索引 = bit 之前 originalMask 的 popcount
            int vi = BitOperations.PopCount(originalMask & ((1UL << bit) - 1));
            result[ri++] = SafeCloneValue(values[vi]);
        }
        return result;
    }

    /// <summary>
    /// 安全克隆值：IGBL 类型深拷贝，值类型已通过装箱复制
    /// </summary>
    private static object SafeCloneValue(object value)
    {
        if (null == value) return null;
        if (value.GetType().IsValueType) return value;
        if (value is IGBL gbl) return gbl.Clone();
        return value;
    }
}
