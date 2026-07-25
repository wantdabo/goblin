using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.BehaviorInfos;

/// <summary>
/// 标签信息, Actor 上的标签信息，用于标记 Actor 各种颗粒度细的信息
/// </summary>
public partial class TagInfo : BehaviorInfo
{
    /// <summary>
    /// 标签的数据集合, 键为 TAG_DEFINE, 值为 Int32
    /// </summary>
    public Dictionary<ushort, long> tags { get; set; }

    protected override void OnReady()
    {
        // 容器只清不还，首次 Ensure，复用时 Reset 已 Clear
        if (null == tags) tags = ObjectCache.Ensure<Dictionary<ushort, long>>();
    }
}
