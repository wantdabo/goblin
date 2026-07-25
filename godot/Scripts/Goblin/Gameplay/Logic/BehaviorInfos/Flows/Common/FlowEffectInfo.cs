using Goblin.Common;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.BehaviorInfos.Flows.Common;

/// <summary>
/// 管线特效信息
/// </summary>
public partial class FlowEffectInfo : BehaviorInfo
{
    /// <summary>
    /// 管线特效 ID 列表
    /// </summary>
    public GBLList<uint> effects { get; set; }
}
