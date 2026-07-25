using Goblin.Common;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.BehaviorInfos.Sa;

/// <summary>
/// 生与死信息
/// </summary>
public partial class SilentMercyInfo : BehaviorInfo
{
    /// <summary>
    /// 出生列表
    /// </summary>
    public GBLList<(ulong actor, ulong flow)> borns { get; set; }
    /// <summary>
    /// 死亡列表
    /// </summary>
    public GBLList<(ulong actor, ulong flow)> deadths { get; set; }
    /// <summary>
    /// 击杀关系, 键为杀手, 值为被杀者
    /// </summary>
    public GBLDict<ulong, GBLList<ulong>> killrelations { get; set; }
    /// <summary>
    /// 受害者关系, 键为被杀者, 值为杀手
    /// </summary>
    public GBLDict<ulong, ulong> victimrelations { get; set; }
}
