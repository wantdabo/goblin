using Goblin.Common;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.BehaviorInfos.Sa;

/// <summary>
/// 事件订阅派发者信息
/// </summary>
public partial class EventorInfo : BehaviorInfo
{
    /// <summary>
    /// 事件订阅派发者的增量计数器, 用于生成唯一的事件索引 (用作排序)
    /// </summary>
    public uint increment { get; set; }

    /// <summary>
    /// 事件索引字典, 用于存储事件的索引 (用作排序)
    /// </summary>
    public GBLDict<(int, ulong actor), uint> indexes { get; set; }
}
