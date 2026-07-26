using Goblin.Gameplay.Projection.Core;
using Goblin.Gameplay.Projection.Rules;

namespace Goblin.Gameplay.Projection;

/// <summary>
/// Observer 工厂 — 按 ObserverType 组装规则链
/// </summary>
public static class ObserverFactory
{
    /// <summary>
    /// 创建 Observer 及其规则链
    /// </summary>
    /// <param name="type">观察者类型</param>
    /// <param name="id">观察者 ID（预留，当前未使用）</param>
    /// <returns>配置好的 Crop 规则链</returns>
    public static Crop CreateRuleChain(ObserverType type, ulong id = 0)
    {
        _ = id;

        var crop = new Crop();
        switch (type)
        {
            case ObserverType.GM:
            case ObserverType.Editor:
            case ObserverType.Replay:
                // 全通过
                crop.AddRule(new GodRule());
                break;

            case ObserverType.Player:
                // 完整裁剪链：AOI → 权限 → 可见性 → 频率
                crop.AddRule(new AOIRule());
                crop.AddRule(new PermissionRule());
                crop.AddRule(new VisibilityRule());
                crop.AddRule(new FrequencyRule());
                break;

            case ObserverType.Spectator:
                // 观战：AOI + 可见性
                crop.AddRule(new AOIRule());
                crop.AddRule(new VisibilityRule());
                break;

            case ObserverType.AI:
                // AI：AOI + 可见性
                crop.AddRule(new AOIRule());
                crop.AddRule(new VisibilityRule());
                break;

            default:
                // 未知观察者类型 — 空规则链（全屏蔽）
                System.Diagnostics.Debug.WriteLine(
                    $"ObserverFactory: 未识别的 ObserverType '{type}'，返回空规则链。请确保新增 ObserverType 后更新本方法。");
                break;
        }

        return crop;
    }
}
