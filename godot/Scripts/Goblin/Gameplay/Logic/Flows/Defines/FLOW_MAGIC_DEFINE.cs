namespace Goblin.Gameplay.Logic.Flows.Defines;

/// <summary>
/// 管线魔法体定义
/// </summary>
public class FLOW_MAGIC_DEFINE
{
    /// <summary>
    /// 生成原点：施法者位置
    /// </summary>
    public const byte BORN_ORIGIN_OWNER = 1;

    /// <summary>
    /// 生成初始旋转：施法者朝向
    /// </summary>
    public const byte BORN_EULER_OWNER = 1;

    /// <summary>
    /// 正前方直线运动
    /// </summary>
    public const ushort MOTION_STRAIGHT = 1;
}