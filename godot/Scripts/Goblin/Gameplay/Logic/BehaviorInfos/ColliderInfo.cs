using Goblin.Gameplay.Logic.Common;
using Kowtow.Math;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.BehaviorInfos;

/// <summary>
/// 碰撞盒
/// </summary>
public partial class ColliderInfo : BehaviorInfo
{
    /// <summary>
    /// 碰撞层
    /// </summary>
    public int layer { get; set; }
    /// <summary>
    /// 几何体类型
    /// </summary>
    public byte shape { get; set; }
    /// <summary>
    /// 立方体
    /// </summary>
    public Box box { get; set; }
    /// <summary>
    /// 球体
    /// </summary>
    public Sphere sphere { get; set; }

    // layer Reset 值为 LAYER_DEFAULT（非 default），SG Reset 设 default 后由此覆盖
    protected override void OnReset()
    {
        layer = COLLISION_DEFINE.LAYER_DEFAULT;
    }
}
