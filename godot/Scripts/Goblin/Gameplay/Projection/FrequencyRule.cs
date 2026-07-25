using System.Collections.Generic;

namespace Goblin.Gameplay.Projection;

/// <summary>
/// 频率裁剪规则 — 每个字段独立推送间隔
/// Phase 1 目前过于激进，此规则后续与 NetworkTransport 配合使用
/// </summary>
public class FrequencyRule : IProjectionRule
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

        var result = currentmask;
        for (var i = 0; i < 64; i++)
        {
            var bit = 1ul << i;
            if (0ul == (currentmask & bit)) continue;

            var key = (packet.behaviorinfotype, i);
            if (false == intervaltable.TryGetValue(key, out var interval)) continue;

            var stateKey = (packet.actor, packet.behaviorinfotype, i);
            if (lastpushtable.TryGetValue(stateKey, out var lastFrame))
            {
                if (packet.frame - lastFrame < interval)
                {
                    result &= ~bit;
                }
            }

            lastpushtable[stateKey] = packet.frame;
        }

        return result;
    }

    /// <summary>
    /// 清理过期帧记录
    /// </summary>
    public void Cleanup(long minFrame)
    {
        var stale = new List<(ulong, System.Type, int)>();
        foreach (var kv in lastpushtable)
        {
            if (kv.Value < minFrame) stale.Add(kv.Key);
        }
        foreach (var key in stale) lastpushtable.Remove(key);
    }
}
