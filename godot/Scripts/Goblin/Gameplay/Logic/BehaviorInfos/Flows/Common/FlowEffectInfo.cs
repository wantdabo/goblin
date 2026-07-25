using System.Collections.Generic;
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
    public List<uint> effects { get; set; }

    protected override void OnReady()
    {
        // 容器只清不还，首次 Ensure，复用时 Reset 已 Clear
        if (null == effects) effects = ObjectCache.Ensure<List<uint>>();
    }
}
