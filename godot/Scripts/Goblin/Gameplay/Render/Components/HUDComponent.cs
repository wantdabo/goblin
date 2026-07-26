using Goblin.Gameplay.Logic.BehaviorInfos;

namespace Goblin.Gameplay.Render.Components;

/// <summary>
/// HUD 组件 — HUDInfo 的纯数据投影
/// </summary>
[ProjectorTarget(typeof(HUDInfo))]
public sealed partial class HUDComponent : Component
{
    /// <summary>
    /// 当前生命值
    /// </summary>
    public int hp { get; set; }

    /// <summary>
    /// 最大生命值
    /// </summary>
    public int maxhp { get; set; }

    /// <summary>
    /// 移动速度
    /// </summary>
    public int movespeed { get; set; }

    /// <summary>
    /// 攻击力
    /// </summary>
    public int attack { get; set; }

}
