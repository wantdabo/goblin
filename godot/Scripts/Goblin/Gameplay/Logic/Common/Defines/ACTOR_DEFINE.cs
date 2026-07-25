using Goblin.Common;
using Goblin.Gameplay.Logic.Common;

namespace Goblin.Gameplay.Logic.Common.Defines;

/// <summary>
/// Actor 定义
/// </summary>
public class ACTOR_DEFINE
{
    /// <summary>
    /// 无
    /// </summary>
    public const byte NONE = 0;
    /// <summary>
    /// 场景
    /// </summary>
    public const byte STAGE = 1;
    /// <summary>
    /// 管线
    /// </summary>
    public const byte FLOW = 2;
    /// <summary>
    /// 英雄
    /// </summary>
    public const byte HERO = 3;
    /// <summary>
    /// 魔法体
    /// </summary>
    public const byte MAGIC = 4;
    /// <summary>
    /// BUFF
    /// </summary>
    public const byte BUFF = 5;
    /// <summary>
    /// 敌人
    /// </summary>
    public const byte ENEMY = 6;

    /// <summary>
    /// 施法者 Actor 类型集合（命中即视为找到施法者）
    /// </summary>
    public static readonly GBLHashSet<byte> CASTER_TYPES = new() { HERO, ENEMY };
}