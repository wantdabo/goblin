using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.BehaviorInfos;

/// <summary>
/// Buff 信息
/// </summary>
public partial class BuffInfo : BehaviorInfo
{
    /// <summary>
    /// BuffID
    /// </summary>
    public int buffid { get; set; }
    /// <summary>
    /// Buff 层数
    /// </summary>
    public int layer { get; set; }
    /// <summary>
    /// Buff 生命周期
    /// </summary>
    public FP lifetime { get; set; }
    /// <summary>
    /// Buff 拥有者
    /// </summary>
    public ulong owner { get; set; }
    /// <summary>
    /// Buff 管线
    /// </summary>
    public ulong flow { get; set; }
    /// <summary>
    /// Buff 是否附魔
    /// </summary>
    public bool enchanted { get; set; }

    protected override void OnReady()
    {
    }
}
