using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.BehaviorInfos;

/// <summary>
/// 魔法体信息
/// </summary>
public partial class MagicInfo : BehaviorInfo
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
    }
}
