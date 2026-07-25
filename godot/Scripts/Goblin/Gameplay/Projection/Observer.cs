namespace Goblin.Gameplay.Projection;

/// <summary>
/// 观察者类型，决定绑定的规则链
/// </summary>
public enum ObserverType
{
    /// <summary>
    /// 普通玩家：完整规则链（AOI + 权限 + 视野 + 频率）
    /// </summary>
    Player,

    /// <summary>
    /// 观战：锁定玩家视角
    /// </summary>
    Spectator,

    /// <summary>
    /// GM：GodRule 全通过
    /// </summary>
    GM,

    /// <summary>
    /// Replay：时间轴驱动
    /// </summary>
    Replay,

    /// <summary>
    /// AI：视野裁剪
    /// </summary>
    AI,

    /// <summary>
    /// 编辑器预览
    /// </summary>
    Editor,
}

/// <summary>
/// 观察者，代表一个数据消费端（玩家/观战/GM/Replay/AI/编辑器）
/// </summary>
public class Observer
{
    /// <summary>
    /// 观察者类型
    /// </summary>
    public ObserverType type { get; set; }

    /// <summary>
    /// 观察者 ID（玩家对应 playerid，GM 对应 0，Replay 对应 replay 实例 ID）
    /// </summary>
    public ulong id { get; set; }
}
