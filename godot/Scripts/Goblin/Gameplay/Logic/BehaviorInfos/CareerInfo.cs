using Goblin.Common;
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
    public GBLList<uint> bornpipelines { get; set; }
    /// <summary>
    /// 死亡管线, Actor 在死亡时会触发这些管线
    /// </summary>
    public GBLList<uint> deathpipelines { get; set; }
}
