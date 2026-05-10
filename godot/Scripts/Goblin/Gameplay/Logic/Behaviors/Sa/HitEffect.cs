using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Behaviors.Sa;

/// <summary>
/// 打击行为
/// </summary>
public class HitEffect : Behavior
{
    protected override void OnTick(FP tick)
    {
        base.OnTick(tick);
        if (false == stage.SeekBehaviorInfos(out List<HitLagInfo> hitlags)) return;
        foreach (var hitlag in hitlags)
        {
            hitlag.elapsed += tick;
            if (hitlag.elapsed < hitlag.duration) continue;
            RmvHitLag(hitlag);
        }
    }

    /// <summary>
    /// 移除顿帧
    /// </summary>
    /// <param name="target">Actor</param>
    public void RmvHitLag(ulong target)
    {
        if (false == stage.SeekBehaviorInfo(target, out HitLagInfo hitlag)) return;
        RmvHitLag(hitlag);
    }

    /// <summary>
    /// 移除顿帧
    /// </summary>
    /// <param name="hitlag">顿帧信息</param>
    public void RmvHitLag(HitLagInfo hitlag)
    {
        hitlag.count--;
        if (hitlag.count > 0) return;

        stage.RmvBehaviorInfo(hitlag);
        if (false == stage.SeekBehaviorInfo(hitlag.actor, out TickerInfo ticker)) return;
        ticker.timescale = hitlag.timescale;
    }

    /// <summary>
    /// 添加顿帧
    /// </summary>
    /// <param name="target">Actor</param>
    /// <param name="strength">顿帧强度</param>
    /// <param name="duration">顿帧持续时间</param>
    public void AddHitLag(ulong target, FP strength, FP duration)
    {
        var isnew = false == stage.SeekBehaviorInfo(target, out HitLagInfo hitlag);
        if (isnew) hitlag = stage.AddBehaviorInfo<HitLagInfo>(target);

        if (hitlag.strength < strength) hitlag.strength = strength;
        if (hitlag.duration < duration) hitlag.duration = duration;
        hitlag.elapsed = FP.Zero;
        hitlag.count++;

        if (hitlag.count > 1) return;

        if (false == stage.SeekBehaviorInfo(target, out TickerInfo ticker)) return;
        hitlag.timescale = ticker.timescale;
        ticker.timescale -= hitlag.strength;
    }
}