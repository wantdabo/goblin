using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.BehaviorInfos;

/// <summary>
/// Actor 职业生涯信息
/// </summary>
public partial class CareerInfo : BehaviorInfo
{
    /// <summary>
    /// 出生管线, Actor 在出生时会触发这些管线
    /// </summary>
    public List<uint> bornpipelines { get; set; }
    /// <summary>
    /// 死亡管线, Actor 在死亡时会触发这些管线
    /// </summary>
    public List<uint> deathpipelines { get; set; }

    protected override void OnReady()
    {
        // 容器只清不还，首次 Ensure，复用时 Reset 已 Clear
        if (null == bornpipelines) bornpipelines = ObjectCache.Ensure<List<uint>>();
        if (null == deathpipelines) deathpipelines = ObjectCache.Ensure<List<uint>>();
    }
}
