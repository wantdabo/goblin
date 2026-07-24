using Goblin.Gameplay.Logic.Core;

namespace Goblin.Logic.Standalone.TestFixtures;

/// <summary>
/// 模式 1：值类型字段
/// SG 生成 override Reset() → value=0, speed=FP.Zero, active=false, base.Reset()
/// </summary>
public partial class SimpleInfo : BehaviorInfo
{
    public int value { get; set; }
    public FP speed { get; set; }
    public bool active2 { get; set; }

    protected override void OnReady()
    {
    }
}
