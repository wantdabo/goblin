using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.BehaviorInfos;

/// <summary>
/// 魔法体信息
/// </summary>
public class MagicInfo : BehaviorInfo
{
    /// <summary>
    /// 施法者 ActorID
    /// </summary>
    public ulong owner { get; set; }
    /// <summary>
    /// 魔法体管线
    /// </summary>
    public ulong flow { get; set; }

    protected override void OnReady()
    {
        OnReset();
    }

    protected override void OnReset()
    {
        owner = 0;
        flow = 0;
    }

    protected override BehaviorInfo OnClone()
    {
        var clone = ObjectCache.Ensure<MagicInfo>();
        clone.Ready(actor);
        clone.owner = owner;
        clone.flow = flow;

        return clone;
    }
}
