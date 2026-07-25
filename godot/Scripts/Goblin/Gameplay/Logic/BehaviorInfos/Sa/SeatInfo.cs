using Goblin.Common;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.BehaviorInfos.Sa;

/// <summary>
/// 座位信息
/// </summary>
public partial class SeatInfo : BehaviorInfo
{
    /// <summary>
    /// 座位字典, 键为座位 ID, 值为 ActorID
    /// </summary>
    public GBLDict<ulong, ulong> sadict { get; set; }
    /// <summary>
    /// 座位字典, 键为 ActorID, 值为座位 ID
    /// </summary>
    public GBLDict<ulong, ulong> asdict { get; set; }
}
