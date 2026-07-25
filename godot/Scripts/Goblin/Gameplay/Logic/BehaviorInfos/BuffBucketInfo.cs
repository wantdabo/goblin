using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.BehaviorInfos;

/// <summary>
/// Buff 桶信息
/// </summary>
public partial class BuffBucketInfo : BehaviorInfo
{
    /// <summary>
    /// Buff 列表
    /// </summary>
    public List<ulong> buffs { get; set; }
    /// <summary>
    /// Buff 字典, 键为 BuffID, 值为 ActorID
    /// </summary>
    public Dictionary<int, ulong> buffdict { get; set; }

    protected override void OnReady()
    {
        // 容器只清不还，首次 Ensure，复用时 Reset 已 Clear
        if (null == buffs) buffs = ObjectCache.Ensure<List<ulong>>();
        if (null == buffdict) buffdict = ObjectCache.Ensure<Dictionary<int, ulong>>();
    }
}
