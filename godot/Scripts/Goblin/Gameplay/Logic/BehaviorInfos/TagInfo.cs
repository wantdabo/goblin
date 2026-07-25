using Goblin.Common;
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
    public GBLDict<ushort, long> tags { get; set; }
}
