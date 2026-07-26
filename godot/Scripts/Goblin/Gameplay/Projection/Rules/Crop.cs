using System;
using System.Collections.Generic;
using Goblin.Common;
using Goblin.Gameplay.Projection.Core;

namespace Goblin.Gameplay.Projection.Rules;

/// <summary>
/// 对象池 key（必须唯一常量，供 ObjectPool 区分）
/// </summary>
public static class CropPoolKey
{
    public const string OBSERVERPACKET_LIST = "CROP_OBSERVERPACKET_LIST";
    public const string VALUE_LIST = "CROP_VALUE_LIST";
}

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
    /// </summary>
    /// <param name="packets">原始数据包数组</param>
    /// <param name="observers">观察者列表</param>
    /// <returns>裁剪后的 ObserverPacket 数组，mask == 0 的已过滤</returns>
    public static ObserverPacket[] Process(ProjectorPacket[] packets, List<Observer> observers)
    {
        if (0 == packets.Length || 0 == observers.Count) return Array.Empty<ObserverPacket>();

        // 从对象池取出 List，清空复用
        var results = ObjectPool.Ensure<List<ObserverPacket>>(CropPoolKey.OBSERVERPACKET_LIST);
        results.Clear();
        foreach (var p in packets)
        {
            foreach (var obs in observers)
            {
                // 使用 Observer 自身的裁剪链（若未设则不裁剪）
                var crop = obs.crop;
                var mask = null != crop ? crop.Project(p, obs) : p.fieldmask;
                if (0 == mask) continue;

                var trimmed = TrimValues(p.values, p.fieldmask, mask);

                // 从对象池取 ObserverPacket 实例，避免每帧 new
                var op = ObjectPool.Ensure<ObserverPacket>(ObserverPacket.POOL_KEY);
                op.observer = obs;
                op.actor = p.actor;
                op.behaviorinfotype = p.behaviorinfotype;
                op.fieldmask = mask;
                op.frame = p.frame;
                op.values = trimmed;
                results.Add(op);
            }
        }
        var array = results.ToArray();
        // 归还 List 容器到对象池
        ObjectPool.Set(results, CropPoolKey.OBSERVERPACKET_LIST);
        return array;
    }

    /// <summary>
    /// 获取规则链中所有规则（用于注入委托）
    /// </summary>
    public IEnumerable<IProjectionRule> GetRules()
    {
        return rules;
    }

    /// <summary>
    /// 按裁剪后 mask 从原始 values 中提取子集
    /// 对引用类型值做安全克隆，防止多线程下逻辑层原地修改导致数据竞争
    /// </summary>
    private static object[] TrimValues(object[] values, ulong originalMask, ulong targetMask)
    {
        if (null == values || 0 == targetMask) return Array.Empty<object>();

        // 从对象池取出 List，清空复用
        var trimmed = ObjectPool.Ensure<List<object>>(CropPoolKey.VALUE_LIST);
        trimmed.Clear();
        var vi = 0;
        for (int bit = 0; bit < 64; bit++)
        {
            if (0 == (originalMask & (1UL << bit))) continue;
            if (0 != (targetMask & (1UL << bit))) trimmed.Add(SafeCloneValue(values[vi]));
            vi++;
        }
        var result = trimmed.ToArray();
        // 归还 List 容器到对象池
        ObjectPool.Set(trimmed, CropPoolKey.VALUE_LIST);
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
