using Kowtow.Math;

namespace Goblin.Gameplay.Render.Components;

/// <summary>
/// 空间组件 — SpatialInfo 的纯数据投影
/// </summary>
public sealed class SpatialComponent : Component
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

    /// <summary>
    /// 应用脏字段 [v1] — 后续迁至 SG 生成
    /// </summary>
    internal static void ApplyTo(object comp, ulong fieldmask, object[] values)
    {
        var c = (SpatialComponent)comp;
        var vi = 0;

        // Bit0: position
        if (0 != (fieldmask & 1)) c.position = (FPVector3)values[vi++];

        // Bit1: euler
        if (0 != (fieldmask & (1ul << 1))) c.euler = (FPVector3)values[vi++];

        // Bit2: scale
        if (0 != (fieldmask & (1ul << 2))) c.scale = (FP)values[vi++];
    }
}
