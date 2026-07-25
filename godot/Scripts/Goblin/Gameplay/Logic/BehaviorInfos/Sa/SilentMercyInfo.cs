using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.BehaviorInfos.Sa;

/// <summary>
/// 生与死信息
/// </summary>
public partial class SilentMercyInfo : BehaviorInfo
{
    /// <summary>
    /// 出生列表
    /// </summary>
    public List<(ulong actor, ulong flow)> borns { get; set; }
    /// <summary>
    /// 死亡列表
    /// </summary>
    public List<(ulong actor, ulong flow)> deadths { get; set; }
    /// <summary>
    /// 击杀关系, 键为杀手, 值为被杀者
    /// </summary>
    public Dictionary<ulong, List<ulong>> killrelations { get; set; }
    /// <summary>
    /// 受害者关系, 键为被杀者, 值为杀手
    /// </summary>
    public Dictionary<ulong, ulong> victimrelations { get; set; }

    protected override void OnReady()
    {
        // 容器只清不还，首次 Ensure，复用时 Reset 已 Clear
        if (null == borns) borns = ObjectCache.Ensure<List<(ulong actor, ulong flow)>>();
        if (null == deadths) deadths = ObjectCache.Ensure<List<(ulong actor, ulong flow)>>();
        if (null == killrelations) killrelations = ObjectCache.Ensure<Dictionary<ulong, List<ulong>>>();
        if (null == victimrelations) victimrelations = ObjectCache.Ensure<Dictionary<ulong, ulong>>();
    }
}
