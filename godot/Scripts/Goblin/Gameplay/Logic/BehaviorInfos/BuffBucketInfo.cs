using Goblin.Common;
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
    public GBLList<ulong> buffs { get; set; }
    /// <summary>
    /// Buff 字典, 键为 BuffID, 值为 ActorID
    /// </summary>
    public GBLDict<int, ulong> buffdict { get; set; }
}
