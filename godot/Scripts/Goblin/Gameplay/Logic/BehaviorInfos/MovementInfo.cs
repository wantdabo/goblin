using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.BehaviorInfos;

/// <summary>
/// 运动信息
/// </summary>
public partial class MovementInfo : BehaviorInfo
{
    /// <summary>
    /// 当前帧驱动了运动, 由 Movement 自身写入(执行后标记)
    /// </summary>
    public bool turnmotion { get; set; }

    protected override void OnReady()
    {
    }
}
