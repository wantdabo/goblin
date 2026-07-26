using System;
using Goblin.Common;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.BehaviorInfos.Sa;

/// <summary>
/// 场景状态
/// </summary>
public enum StageState
{
    /// <summary>
    /// 无
    /// </summary>
    None,
    /// <summary>
    /// 初始化
    /// </summary>
    Initialized,
    /// <summary>
    /// 销毁了
    /// </summary>
    Disposed,
    /// <summary>
    /// 暂停中
    /// </summary>
    Paused,
    /// <summary>
    /// 驱动中
    /// </summary>
    Ticking,
    /// <summary>
    /// 停止了
    /// </summary>
    Stopped,
}

/// <summary>
/// 场景信息
/// </summary>
public partial class StageInfo : BehaviorInfo
{
    /// <summary>
    /// 当前 Stage 状态
    /// </summary>
    public StageState state { get; set; }
    /// <summary>
    /// 帧号
    /// </summary>
    public uint frame { get; set; }
    /// <summary>
    /// 流逝时间
    /// </summary>
    public FP elapsed { get; set; }
    /// <summary>
    /// 时间缩放
    /// </summary>
    public FP timescale { get; set; }
    /// <summary>
    /// Actor 自增 ID
    /// </summary>
    public ulong increment { get; set; }
    /// <summary>
    /// Actor 列表
    /// </summary>
    public GBLList<ulong> actors { get; set; }
    /// <summary>
    /// 行为类型列表, 键为 ActorID, 值为该 Actor 上的所有行为类型
    /// </summary>
    public GBLDict<ulong, GBLList<Type>> behaviortypes { get; set; }
    /// <summary>
    /// 行为信息列表, 键为行为类型, 值为该行为类型的所有 BehaviorInfo 列表
    /// </summary>
    public GBLDict<Type, GBLList<BehaviorInfo>> behaviorinfos { get; set; }

    protected override void OnReady()
    {
        base.OnReady();
        timescale = FP.One;
    }

    protected override void OnReset()
    {
        base.OnReset();
        timescale = FP.One;
    }
}
