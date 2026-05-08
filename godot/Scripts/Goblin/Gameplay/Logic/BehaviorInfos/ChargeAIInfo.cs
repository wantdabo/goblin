using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.BehaviorInfos;

/// <summary>
/// 直冲怪 AI 信息
/// </summary>
public class ChargeAIInfo : BehaviorInfo
{
    /// <summary>
    /// 攻击距离, 进入此距离内 AI 停下
    /// </summary>
    public FP attackrange { get; set; }
    /// <summary>
    /// 当前锁定的目标 ActorID, 0 表示无目标
    /// </summary>
    public ulong target { get; set; }

    protected override void OnReady()
    {
        OnReset();
    }

    protected override void OnReset()
    {
        attackrange = FP.Zero;
        target = 0;
    }

    protected override BehaviorInfo OnClone()
    {
        var clone = ObjectCache.Ensure<ChargeAIInfo>();
        clone.Ready(actor);
        clone.attackrange = attackrange;
        clone.target = target;

        return clone;
    }
}
