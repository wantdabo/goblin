using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.BehaviorInfos;

/// <summary>
/// 状态机信息
/// </summary>
public partial class StateMachineInfo : BehaviorInfo
{
    /// <summary>
    /// 当前状态
    /// </summary>
    public byte current { get; set; }
    /// <summary>
    /// 上一个状态
    /// </summary>
    public byte last { get; set; }
    /// <summary>
    /// 是否使用延迟中断状态
    /// </summary>
    public bool usedelaybreak { get; set; }
    /// <summary>
    /// 延迟中断时间
    /// </summary>
    public FP delaybreak { get; set; }
    /// <summary>
    /// 状态已持续时间
    /// </summary>
    public FP stateduration { get; set; }
    /// <summary>
    /// 限时状态结束后的回退状态
    /// </summary>
    public byte timerfallback { get; set; }
}
