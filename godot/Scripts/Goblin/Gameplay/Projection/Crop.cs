using System;
using System.Collections.Generic;

namespace Goblin.Gameplay.Projection;

/// <summary>
/// 裁剪规则链 — 串联多个 IProjectionRule，逐步修剪 fieldmask
/// mask == 0 时丢弃整条数据包
/// </summary>
public class Crop
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
    /// <param name="crop">裁剪实例</param>
    /// <returns>裁剪后的 ObserverPacket 数组，mask == 0 的已过滤</returns>
    public static ObserverPacket[] Process(ProjectorPacket[] packets, List<Observer> observers, Crop crop)
    {
        if (0 == packets.Length || 0 == observers.Count) return Array.Empty<ObserverPacket>();

        var results = new List<ObserverPacket>();
        foreach (var p in packets)
        {
            foreach (var obs in observers)
            {
                var mask = crop.Project(p, obs);
                if (0 == mask) continue;

                results.Add(new ObserverPacket
                {
                    observer = obs,
                    actor = p.actor,
                    behaviorinfotype = p.behaviorinfotype,
                    fieldmask = mask,
                    frame = p.frame,
                    values = p.values,
                });
            }
        }
        return results.ToArray();
    }
}

/// <summary>
/// 全通过规则 — Phase 1 所有 Observer 挂此规则（零裁剪）
/// </summary>
public class GodRule : IProjectionRule
{
    /// <summary>
    /// 全通过，不修剪任何字段
    /// </summary>
    public ulong Filter(ProjectorPacket packet, Observer observer, ulong currentmask)
    {
        return currentmask;
    }
}
