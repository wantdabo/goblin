namespace Goblin.Gameplay.Projection;

/// <summary>
/// 可见性裁剪规则 — 不可见实体返回 0
/// </summary>
public class VisibilityRule : IProjectionRule
{
    /// <summary>
    /// 可见性查询委托（由 RenderWorld/GameLogic 注入）
    /// 返回 false 表示不可见
    /// </summary>
    public System.Func<ulong, bool> visibilitylookup { get; set; }

    /// <summary>
    /// 裁剪：不可见返回 0
    /// </summary>
    public ulong Filter(ProjectorPacket packet, Observer observer, ulong currentmask)
    {
        if (0 == currentmask) return 0;
        if (null == visibilitylookup) return currentmask;

        if (false == visibilitylookup(packet.actor)) return 0;

        return currentmask;
    }
}
