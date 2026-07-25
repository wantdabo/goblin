using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.BehaviorInfos;

/// <summary>
/// 顿帧信息
/// </summary>
public partial class HitLagInfo : BehaviorInfo
{
    /// <summary>
    /// 修改前的时间缩放
    /// </summary>
    public FP timescale { get; set; }
    /// <summary>
    /// 强度
    /// </summary>
    public FP strength { get; set; }
    /// <summary>
    /// 持续时间
    /// </summary>
    public FP duration { get; set; }
    /// <summary>
    /// 已流逝时间
    /// </summary>
    public FP elapsed { get; set; }
    /// <summary>
    /// 引用计数，保证 timescale 只被保存/恢复一次
    /// </summary>
    public int count { get; set; }

    protected override void OnReady()
    {
    }
}
