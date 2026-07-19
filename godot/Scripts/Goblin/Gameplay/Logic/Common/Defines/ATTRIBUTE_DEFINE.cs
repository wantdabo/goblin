namespace Goblin.Gameplay.Logic.Common.Defines;

/// <summary>
/// 属性定义
/// </summary>
public class ATTRIBUTE_DEFINE
{
    /// <summary>
    /// 当前生命值
    /// </summary>
    public const ushort HP = 1;
    /// <summary>
    /// 最大生命值
    /// </summary>
    public const ushort MAXHP = 2;
    /// <summary>
    /// 移动速度
    /// </summary>
    public const ushort MOVESPEED = 3;
    /// <summary>
    /// 攻击力
    /// </summary>
    public const ushort ATTACK = 4;
    /// <summary>
    /// 护甲（固定值减伤）
    /// </summary>
    public const ushort ARMOR = 5;
    /// <summary>
    /// 魔法抗性（固定值魔伤减免）
    /// </summary>
    public const ushort MAGIC_RESIST = 6;
    /// <summary>
    /// 暴击率（千分比，500 = 50%）
    /// </summary>
    public const ushort CRIT_RATE = 7;
    /// <summary>
    /// 闪避率（千分比，200 = 20%）
    /// </summary>
    public const ushort DODGE_RATE = 8;
}