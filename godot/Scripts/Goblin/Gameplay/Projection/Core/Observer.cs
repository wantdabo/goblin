using Kowtow.Math;
using Goblin.Gameplay.Projection.Rules;

namespace Goblin.Gameplay.Projection.Core;

/// <summary>
/// 观察者类型 — 决定裁剪规则链
/// </summary>
public enum ObserverType
{
    GM,
    Editor,
    Replay,
    Player,
    Spectator,
    AI,
}

/// <summary>
/// 观察者 — 代表一个数据消费端
/// 包含观察者身份、AOI 半径、关注的目标 Actor
/// </summary>
public class Observer
{
    /// <summary>
    /// 观察者类型
    /// </summary>
    public ObserverType type { get; set; }

    /// <summary>
    /// 观察者 ActorID
    /// </summary>
    public ulong id { get; set; }

    /// <summary>
    /// 关注的目标 Actor（AOI 中心），null 表示全局
    /// </summary>
    public ulong? observedActor { get; set; }

    /// <summary>
    /// AOI 半径
    /// </summary>
    public FP radius { get; set; }

    /// <summary>
    /// 裁剪规则链
    /// </summary>
    public Crop crop { get; set; }
}
