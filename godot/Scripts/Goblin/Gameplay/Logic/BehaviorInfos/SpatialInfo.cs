using Goblin.Common;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.BehaviorInfos;

// 位置
[Projector("position", typeof(FPVector3), 0)]
// 旋转
[Projector("euler", typeof(FPVector3), 1)]
// 缩放（Reset 时归 FP.One）
[Projector("scale", typeof(FP), 2, defaultvalue = 1)]
/// <summary>
/// 空间信息
/// </summary>
public partial class SpatialInfo : BehaviorInfo
{
    /// <summary>
    /// 上一帧位置, 旋转, 缩放
    /// </summary>
    public (FPVector3 position, FPVector3 euler, FP scale) preframe { get; set; }

    protected override void OnReady()
    {
        OnReset();
    }

    protected override void OnReset()
    {
        // position/euler/scale 由 SG 生成的 Reset 接管，此处仅重置非投影字段
        preframe = (FPVector3.zero, FPVector3.zero, FP.One);
    }
}
