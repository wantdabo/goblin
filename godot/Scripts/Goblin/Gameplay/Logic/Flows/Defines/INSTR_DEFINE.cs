namespace Goblin.Gameplay.Logic.Flows.Defines;

/// <summary>
/// 指令定义
/// </summary>
public class INSTR_DEFINE
{
    /// <summary>
    /// 动画指令
    /// </summary>
    public const ushort ANIMATION = 1;
    /// <summary>
    /// POSITION 变化指令
    /// </summary>
    public const ushort SPATIAL_POSITION = 2;
    /// <summary>
    /// 生成魔法体指令
    /// </summary>
    public const ushort CREATE_MAGIC = 3;
    /// <summary>
    /// 释放技能
    /// </summary>
    public const ushort LAUNCH_SKILL = 4;
    /// <summary>
    /// 特效指令
    /// </summary>
    public const ushort EFFECT = 5;
    /// <summary>
    /// 碰撞指令
    /// </summary>
    public const ushort COLLISION = 6;
    /// <summary>
    /// 移除 Actor 指令
    /// </summary>
    public const ushort RMV_ACTOR = 7;
    /// <summary>
    /// 状态变化指令
    /// </summary>
    public const ushort CHANGE_STATE = 8;
    /// <summary>
    /// 火花指令
    /// </summary>
    public const ushort SPARK = 9;
    /// <summary>
    /// 顿帧指令
    /// </summary>
    public const ushort HIT_LAG = 10;
    /// <summary>
    /// 时间缩放指令
    /// </summary>
    public const ushort TIMESCALE = 11;
    /// <summary>
    /// 受击指令
    /// </summary>
    public const ushort BEHIT = 12;
    /// <summary>
    /// 技能打断指令
    /// </summary>
    public const ushort SKILLBREAK = 13;
    /// <summary>
    /// 伤害结算指令
    /// </summary>
    public const ushort DAMAGE = 14;
    /// <summary>
    /// 音效指令
    /// </summary>
    public const ushort SOUND = 15;
}