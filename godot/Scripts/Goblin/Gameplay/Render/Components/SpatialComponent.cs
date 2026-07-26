using Goblin.Gameplay.Logic.BehaviorInfos;
using Kowtow.Math;

namespace Goblin.Gameplay.Render.Components;

/// <summary>
/// 空间组件 — SpatialInfo 的纯数据投影
/// </summary>
[ProjectorTarget(typeof(SpatialInfo))]
public sealed partial class SpatialComponent : Component
{
    /// <summary>
    /// 世界坐标
    /// </summary>
    public FPVector3 position { get; set; }

    /// <summary>
    /// 欧拉角旋转
    /// </summary>
    public FPVector3 euler { get; set; }

    /// <summary>
    /// 缩放
    /// </summary>
    public FP scale { get; set; } = FP.One;
}
