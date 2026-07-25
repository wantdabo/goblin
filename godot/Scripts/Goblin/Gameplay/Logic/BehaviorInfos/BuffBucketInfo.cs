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
    /// Buff 字典, 键为 BuffID, 值为 ActorID
    /// GBLDict 内部维护插入顺序，可直接有序遍历 Values
    /// </summary>
    public GBLDict<int, ulong> buffdict { get; set; }
}
