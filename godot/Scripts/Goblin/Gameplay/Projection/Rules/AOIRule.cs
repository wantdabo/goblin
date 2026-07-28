using System;
using Kowtow.Math;
using Goblin.Gameplay.Projection.Core;

namespace Goblin.Gameplay.Projection.Rules;

/// <summary>
/// AOI 裁剪规则 — 距离过滤，超出观察者半径返回 0
/// Fail-open 设计：位置查询不可用时（positionlookup 未注入 / actor 无位置），放行所有数据
/// 适用场景：初始化期、非空间场景，确保数据不断流
/// </summary>
public class AOIRule : IProjectionRule
{
    /// <summary>
    /// Actor 位置查询委托（由 Canvas 注入，查找 actor 的 SpatialShadow 位置）
    /// </summary>
    public Func<ulong, FPVector3?>? positionlookup { get; set; }

    /// <summary>
    /// 裁剪：超出 Observer 的 AOI 半径返回 0
    /// </summary>
    public ulong Filter(ProjectorPacket packet, Observer observer, ulong currentmask)
    {
        if (null == positionlookup) return currentmask;
        if (null == observer.observedactor) return currentmask;

        var center = positionlookup(observer.observedactor.Value);
        if (null == center) return currentmask;

        var target = positionlookup(packet.actor);
        if (null == target) return currentmask;

        var rad = observer.radius;
        if (rad <= FP.Zero) return currentmask;

        var sqr = (center.Value - target.Value).sqrMagnitude;
        if (sqr <= rad * rad) return currentmask;

        return 0;
    }
}
