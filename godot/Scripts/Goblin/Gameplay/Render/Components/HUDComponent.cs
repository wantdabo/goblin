namespace Goblin.Gameplay.Render.Components;

/// <summary>
/// HUD 组件 — HUDInfo 的纯数据投影
/// </summary>
public sealed class HUDComponent : Component
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

    /// <summary>
    /// 应用脏字段 [v1] — 后续迁至 SG 生成
    /// </summary>
    internal static void ApplyTo(object comp, ulong fieldmask, object[] values)
    {
        var c = (HUDComponent)comp;
        var vi = 0;

        // Bit0: hp
        if (0 != (fieldmask & 1)) c.hp = (int)values[vi++];

        // Bit1: maxhp
        if (0 != (fieldmask & (1ul << 1))) c.maxhp = (int)values[vi++];

        // Bit2: movespeed
        if (0 != (fieldmask & (1ul << 2))) c.movespeed = (int)values[vi++];

        // Bit3: attack
        if (0 != (fieldmask & (1ul << 3))) c.attack = (int)values[vi++];
    }
}
