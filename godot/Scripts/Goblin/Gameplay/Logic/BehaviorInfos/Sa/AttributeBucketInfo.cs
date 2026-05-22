using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.BehaviorInfos.Sa;

/// <summary>
/// 属性桶信息（Sa 级，统一管理所有 Actor 的属性）
/// </summary>
public class AttributeBucketInfo : BehaviorInfo
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
        attributes = ObjectCache.Ensure<Dictionary<ulong, Dictionary<ushort, int>>>();
        pendings = ObjectCache.Ensure<List<ulong>>();
    }

    protected override void OnReset()
    {
        foreach (var kv in attributes)
        {
            kv.Value.Clear();
            ObjectCache.Set(kv.Value);
        }
        attributes.Clear();
        ObjectCache.Set(attributes);

        pendings.Clear();
        ObjectCache.Set(pendings);
    }

    protected override BehaviorInfo OnClone()
    {
        var clone = ObjectCache.Ensure<AttributeBucketInfo>();
        clone.Ready(actor);
        foreach (var kv in attributes)
        {
            var dict = ObjectCache.Ensure<Dictionary<ushort, int>>();
            foreach (var kv2 in kv.Value) dict.Add(kv2.Key, kv2.Value);
            clone.attributes.Add(kv.Key, dict);
        }
        clone.pendings.AddRange(pendings);

        return clone;
    }
}
