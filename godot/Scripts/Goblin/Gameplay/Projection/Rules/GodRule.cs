using Goblin.Gameplay.Projection.Core;

namespace Goblin.Gameplay.Projection.Rules;

/// <summary>
/// 全通过规则 — Phase 1 所有 Observer 挂此规则（零裁剪）
/// </summary>
public class GodRule : IProjectionRule
{
    /// <summary>
    /// 全通过，不修剪任何字段
    /// </summary>
    public ulong Filter(ProjectorPacket packet, Observer observer, ulong currentmask)
    {
        return currentmask;
    }
}
