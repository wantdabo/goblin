using System.Collections.Generic;

namespace Goblin.Gameplay.Logic.Common.Defines;

/// <summary>
/// State 定义
/// </summary>
public class STATE_DEFINE
{
    /// <summary>
    /// 无
    /// </summary>
    public const byte NONE = 0;
    /// <summary>
    /// 出生
    /// </summary>
    public const byte BORN = 1;
    /// <summary>
    /// 死亡
    /// </summary>
    public const byte DEATH = 2;
    /// <summary>
    /// 待机
    /// </summary>
    public const byte IDLE = 3;
    /// <summary>
    /// 移动
    /// </summary>
    public const byte MOVE = 4;
    /// <summary>
    /// 跳跃
    /// </summary>
    public const byte JUMP = 5;
    /// <summary>
    /// 下坠
    /// </summary>
    public const byte FALL = 6;
    /// <summary>
    /// 技能
    /// </summary>
    public const byte CASTING = 7;
    /// <summary>
    /// 硬直
    /// </summary>
    public const byte HITSTUN = 8;
    /// <summary>
    /// 翻滚（无敌帧，不可被 HITSTUN 打断）
    /// </summary>
    public const byte ROLL = 9;

    /// <summary>
    /// 状态切换规则
    /// </summary>
    public static Dictionary<byte, List<byte>> PASSES { get; private set; } = new()
    {
        { BORN, new List<byte>() { } },
        { DEATH, new List<byte>() { } },
        { IDLE, new List<byte>() { DEATH, MOVE, FALL, CASTING, HITSTUN, ROLL } },
        { MOVE, new List<byte>() { DEATH, IDLE, FALL, CASTING, HITSTUN, ROLL } },
        { JUMP, new List<byte>() { DEATH, FALL, CASTING, HITSTUN } },
        { FALL, new List<byte>() { DEATH, IDLE, CASTING, HITSTUN } },
        { CASTING, new List<byte>() { DEATH, HITSTUN } },
        { HITSTUN, new List<byte>() { DEATH, HITSTUN } },
        { ROLL, new List<byte>() { DEATH, IDLE, MOVE, CASTING } },
    };
}