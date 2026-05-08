using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.BehaviorInfos;

/// <summary>
/// 运动信息
/// </summary>
public class MovementInfo : BehaviorInfo
{
    /// <summary>
    /// 期望移动方向(归一化), 由 Pilot/AI 等驱动者写入
    /// </summary>
    public FPVector3 dire { get; set; }
    /// <summary>
    /// 这帧是否想移动, 由 Pilot/AI 等驱动者写入
    /// </summary>
    public bool wantmove { get; set; }
    /// <summary>
    /// 当前帧驱动了运动, 由 Movement 自身写入(执行后标记)
    /// </summary>
    public bool turnmotion { get; set; }

    protected override void OnReady()
    {
        OnReset();
    }

    protected override void OnReset()
    {
        dire = FPVector3.zero;
        wantmove = false;
        turnmotion = false;
    }

    protected override BehaviorInfo OnClone()
    {
        var clone = ObjectCache.Ensure<MovementInfo>();
        clone.Ready(actor);
        clone.dire = dire;
        clone.wantmove = wantmove;
        clone.turnmotion = turnmotion;

        return clone;
    }
}