using Goblin.Common;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.BehaviorInfos;

// 位置
[Projector("position", typeof(FPVector3))]
// 旋转
[Projector("euler", typeof(FPVector3))]
// 缩放
[Projector("scale", typeof(FP))]
/// <summary>
/// 空间信息
/// </summary>
public partial class SpatialInfo : BehaviorInfo
{
    /// <summary>
    /// 上一帧位置, 旋转, 缩放
    /// </summary>
    public (FPVector3 position, FPVector3 euler, FP scale) preframe { get; set; }
}
