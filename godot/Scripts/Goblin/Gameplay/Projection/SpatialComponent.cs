using Goblin.Common;
using Kowtow.Math;

namespace Goblin.Gameplay.Projection;

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
    /// 应用脏字段
    /// </summary>
    public override void Apply(ulong fieldmask, object[] values)
    {
        var vi = 0;

        // Bit0: position
        // Bit1: euler
        // Bit2: scale
        if (0 != (fieldmask & 1))
        {
            position = (FPVector3)values[vi++];
        }

        if (0 != (fieldmask & (1ul << 1)))
        {
            euler = (FPVector3)values[vi++];
        }

        if (0 != (fieldmask & (1ul << 2)))
        {
            scale = (FP)values[vi++];
        }
    }
}
