using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.Behaviors;

/// <summary>
/// 标签系统（Sa 级）
/// 管理所有 Actor 的标签数据
/// </summary>
public class Tag : Behavior
{
    /// <summary>
    /// 设置标签
    /// </summary>
    public void Set(ulong actor, ushort key, int tag)
    {
        var taginfo = stage.GetBehaviorInfo<TagInfo>(actor, true);
        if (taginfo.tags.ContainsKey(key)) taginfo.tags.Remove(key);
        taginfo.tags.Add(key, tag);
    }

    /// <summary>
    /// 获取标签
    /// </summary>
    public long Get(ulong actor, ushort key)
    {
        if (false == stage.SeekBehaviorInfo(actor, out TagInfo taginfo)) return 0;
        if (false == taginfo.tags.TryGetValue(key, out var tag)) return 0;
        return tag;
    }

    /// <summary>
    /// 检查标签是否存在
    /// </summary>
    public bool Has(ulong actor, ushort key)
    {
        if (false == stage.SeekBehaviorInfo(actor, out TagInfo taginfo)) return false;
        return taginfo.tags.ContainsKey(key);
    }

    /// <summary>
    /// 移除标签
    /// </summary>
    public void Rmv(ulong actor, ushort key)
    {
        if (false == stage.SeekBehaviorInfo(actor, out TagInfo taginfo)) return;
        if (false == taginfo.tags.TryGetValue(key, out long tag)) return;
        taginfo.tags.Remove(key);
    }
}
