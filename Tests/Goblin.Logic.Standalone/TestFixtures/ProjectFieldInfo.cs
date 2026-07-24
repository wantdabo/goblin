using Goblin.Gameplay.Logic.Core;

namespace Goblin.Logic.Standalone.TestFixtures;

/// <summary>
/// 模式 4：含 [Projector] 字段
/// [Projector] 注解在 T1.1 添加后 SG 生成 property + dirty mask + override Reset()
/// 测试：Reset → scale==FP.One（非 FP.Zero）、projectdirtymask==0
/// </summary>
public partial class ProjectFieldInfo : BehaviorInfo
{
    // [Projector(index: 0)] — 激活于 T1.1
    public FPVector3 position { get; set; }

    // [Projector(index: 1, default: 1)] — 激活于 T1.1
    public FP scale { get; set; }

    public string name { get; set; }

    protected override void OnReady()
    {
    }
}
