using Goblin.Gameplay.Projection.Core;

namespace Goblin.Gameplay.Projection.Rules;

/// <summary>
/// 可见性裁剪规则 — 不可见实体的字段按 visiblefield 掩码裁剪
/// visibilitylookup 须反映游戏层可见性（隐身、战争迷雾等），而非数据存在性
/// 当前 Phase 1 用 Canvas.HasActor 做占位实现
/// </summary>
public class VisibilityRule : IProjectionRule
{
    /// <summary>
    /// 可见性查询委托（由 Canvas / GameLogic 注入）
    /// 返回 false 表示不可见
    /// </summary>
    public System.Func<ulong, bool>? visibilitylookup { get; set; }

    /// <summary>
    /// 裁剪：不可见返回 0
    /// Fail-open：首次同步时 Canvas 尚无该 Actor 数据，放行让数据流入
    /// </summary>
    public ulong Filter(ProjectorPacket packet, Observer observer, ulong currentmask)
    {
        if (0 == currentmask) return 0;
        if (null == visibilitylookup) return currentmask;

        // Canvas 中尚无该 Actor → 首帧数据，放行
        if (false == visibilitylookup(packet.actor)) return currentmask;

        return currentmask;
    }
}
