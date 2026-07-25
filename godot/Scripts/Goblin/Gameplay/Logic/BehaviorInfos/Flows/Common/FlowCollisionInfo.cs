using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.BehaviorInfos.Flows.Common;

/// <summary>
/// Flow 碰撞信息基类
/// </summary>
public abstract partial class FlowCollisionInfo : BehaviorInfo
{
    /// <summary>
    /// 碰撞记录
    /// </summary>
    public Dictionary<(uint pipeline, uint index), Dictionary<ulong, uint>> records { get; set; }
    /// <summary>
    /// 碰撞的 ActorID 列表
    /// </summary>
    public List<(ulong actor, (uint pipeline, uint index) identity)> targets { get; set; }

    protected override void OnReady()
    {
        records = ObjectCache.Ensure<Dictionary<(uint pipeline, uint index), Dictionary<ulong, uint>>>();
        targets = ObjectCache.Ensure<List<(ulong actor, (uint pipeline, uint index) identity)>>();
    }
}
