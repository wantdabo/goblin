using Goblin.Common;
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
    public GBLDict<(uint pipeline, uint index), GBLDict<ulong, uint>> records { get; set; }
    /// <summary>
    /// 碰撞的 ActorID 列表
    /// </summary>
    public GBLList<(ulong actor, (uint pipeline, uint index) identity)> targets { get; set; }
}
