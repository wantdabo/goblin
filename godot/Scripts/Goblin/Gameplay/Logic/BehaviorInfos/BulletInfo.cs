using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.BehaviorInfos;

/// <summary>
/// 子弹信息
/// </summary>
public class BulletInfo : BehaviorInfo
{
    /// <summary>
    /// 子弹拥有者
    /// </summary>
    public ulong owner { get; set; }
    /// <summary>
    /// 子弹管线
    /// </summary>
    public ulong flow { get; set; }
    /// <summary>
    /// 子弹伤害强度
    /// </summary>
    public FP strength { get; set; }
    /// <summary>
    /// 子弹的速度
    /// </summary>
    public FP speed { get; set; }

    protected override void OnReady()
    {
        OnReset();
    }

    protected override void OnReset()
    {
        owner = 0;
        flow = 0;
        strength = 0;
        speed = 0;
    }

    protected override BehaviorInfo OnClone()
    {
        var clone = ObjectCache.Ensure<BulletInfo>();
        clone.Ready(actor);
        clone.owner = owner;
        clone.flow = flow;
        clone.strength = strength;
        clone.speed = speed;

        return clone;
    }
}