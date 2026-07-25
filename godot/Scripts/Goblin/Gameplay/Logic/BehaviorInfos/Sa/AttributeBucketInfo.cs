using System.Collections.Generic;
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
    public Dictionary<ulong, Dictionary<ushort, int>> attributes { get; set; }
    /// <summary>
    /// 已死亡、等待依赖检查后回收的 actor 列表
    /// </summary>
    public List<ulong> pendings { get; set; }

    protected override void OnReady()
    {
        // 容器只清不还，首次 Ensure，复用时 Reset 已 Clear
        if (null == attributes) attributes = ObjectCache.Ensure<Dictionary<ulong, Dictionary<ushort, int>>>();
        if (null == pendings) pendings = ObjectCache.Ensure<List<ulong>>();
    }
}
