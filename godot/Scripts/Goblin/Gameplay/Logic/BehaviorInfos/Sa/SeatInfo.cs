using System.Collections.Generic;
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
    public Dictionary<ulong, ulong> sadict { get; set; }
    /// <summary>
    /// 座位字典, 键为 ActorID, 值为座位 ID
    /// </summary>
    public Dictionary<ulong, ulong> asdict { get; set; }

    protected override void OnReady()
    {
        // 容器只清不还，首次 Ensure，复用时 Reset 已 Clear
        if (null == sadict) sadict = ObjectCache.Ensure<Dictionary<ulong, ulong>>();
        if (null == asdict) asdict = ObjectCache.Ensure<Dictionary<ulong, ulong>>();
    }
}
