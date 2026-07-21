namespace Goblin.Gameplay.Logic.Common.Defines;

/// <summary>
/// 动画定义
/// </summary>
public class ANIM_DEFINE
{
    /// <summary>
    /// 自动播放
    /// </summary>
    public const byte TICK_AUTOMATIC = 0;
    /// <summary>
    /// 手动播放
    /// </summary>
    public const byte TICK_MANUAL = 1;

    /// <summary>
    /// StateMachine 基态
    /// </summary>
    public const byte SLOT_TYPE_STATE = 0;
    /// <summary>
    /// AnimationData 命名动画
    /// </summary>
    public const byte SLOT_TYPE_NAMED = 1;
    /// <summary>
    /// 行为覆盖（BeHitExecutor 等写入）
    /// </summary>
    public const byte SLOT_TYPE_OVERRIDE = 2;

    /// <summary>
    /// 构造复合槽位键（高字节=类型，低字节=层）
    /// </summary>
    public static ushort GenKey(byte slottype, byte layer) => (ushort)((slottype << 8) | layer);

    /// <summary>
    /// 从复合键提取槽位类型
    /// </summary>
    public static byte GetSlotType(ushort key) => (byte)(key >> 8);

    /// <summary>
    /// 从复合键提取层
    /// </summary>
    public static byte GetSlotLayer(ushort key) => (byte)(key & 0xFF);

    /// <summary>
    /// 优先级：基础运动
    /// </summary>
    public const int SLOT_PRIORITY_LOCOMOTION = 0;
    /// <summary>
    /// 优先级：交互动作
    /// </summary>
    public const int SLOT_PRIORITY_INTERACT = 100;
    /// <summary>
    /// 优先级：主动动作
    /// </summary>
    public const int SLOT_PRIORITY_ACTION = 200;
    /// <summary>
    /// 优先级：受击反应
    /// </summary>
    public const int SLOT_PRIORITY_REACTION = 400;
    /// <summary>
    /// 优先级：反击
    /// </summary>
    public const int SLOT_PRIORITY_COUNTER = 500;
    /// <summary>
    /// 优先级：击倒
    /// </summary>
    public const int SLOT_PRIORITY_KNOCKDOWN = 600;
    /// <summary>
    /// 优先级：硬控
    /// </summary>
    public const int SLOT_PRIORITY_HARDCROWD = 700;
    /// <summary>
    /// 优先级：生命状态
    /// </summary>
    public const int SLOT_PRIORITY_LIFESTATE = 800;
    /// <summary>
    /// 优先级：系统接管
    /// </summary>
    public const int SLOT_PRIORITY_SYSTEM = 1000;

    /// <summary>
    /// 动画层：全身
    /// </summary>
    public const byte LAYER_FULLBODY = 0;
    /// <summary>
    /// 动画层：上半身
    /// </summary>
    public const byte LAYER_UPPER = 1;
    /// <summary>
    /// 动画层：下半身
    /// </summary>
    public const byte LAYER_LOWER = 2;
    /// <summary>
    /// 最大动画层数
    /// </summary>
    public const byte LAYER_MAX = 3;
}