namespace Goblin.Gameplay.Logic.Flows.Defines;

/// <summary>
/// 管线定义
/// </summary>
public class FLOW_DEFINE
{
    /// <summary>
    /// 管线长度最大值
    /// </summary>
    public const ulong MAX_LENGTH = ulong.MaxValue / 2;
    /// <summary>
    /// 管线长度最大值 - 溢出
    /// </summary>
    public const ulong OVERFLOW_LENGTH = ulong.MaxValue;

    /// <summary>
    /// 执行目标 - 管线
    /// </summary>
    public const byte ET_FLOW = 1;
    /// <summary>
    /// 执行目标 - 管线拥有者
    /// </summary>
    public const byte ET_FLOW_OWNER = 2;
    /// <summary>
    /// 执行目标 - 管线命中
    /// </summary>
    public const byte ET_FLOW_HIT = 3;
        
    /// <summary>
    /// 脚本 ID 10000 — 出生（切换到 IDLE）
    /// </summary>
    public const uint S10000 = 10000;
    /// <summary>
    /// 脚本 ID 10001 — 死亡（切换到 DEATH 并销毁）
    /// </summary>
    public const uint S10001 = 10001;
    /// <summary>
    /// 脚本 ID 100000001
    /// </summary>
    public const uint S100000001 = 100000001;
    /// <summary>
    /// 脚本 ID 100000002
    /// </summary>
    public const uint S100000002 = 100000002;
    /// <summary>
    /// 脚本 ID 10020 — 翻滚
    /// </summary>
    public const uint S10010 = 10010;
    /// <summary>
    /// 脚本 ID 10030 — 重击
    /// </summary>
    public const uint S10020 = 10020;

    /// <summary>
    /// 指令条件不满足重试递归上限
    /// </summary>
    public const int MAX_INSIDE_NOTEXE_DEPTH = 10;
}