using Goblin.Common;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.BehaviorInfos.Sa;

/// <summary>
/// 属性桶信息（Sa 级，统一管理所有 Actor 的属性）
/// </summary>
public partial class AttributeBucketInfo : BehaviorInfo
{
    /// <summary>
    /// actor → 属性数据 (attrkey → value)
    /// </summary>
    public GBLDict<ulong, GBLDict<ushort, int>> attributes { get; set; }
    /// <summary>
    /// 已死亡、等待依赖检查后回收的 actor 列表
    /// </summary>
    public GBLList<ulong> pendings { get; set; }
}
