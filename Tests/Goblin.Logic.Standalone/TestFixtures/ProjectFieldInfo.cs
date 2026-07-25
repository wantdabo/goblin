using Goblin.Common;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Logic.Standalone.TestFixtures;

/// <summary>
/// 模式 4：含 [Projector] 字段
/// [Projector] 类级注解，SG 生成 backing field + 脏标记属性
/// 测试：Reset → scale==FP.One（非 FP.Zero）、projectdirtymask==0
/// </summary>
// 角色世界坐标
[Projector("position", typeof(FPVector3), 0)]
// 模型缩放
[Projector("scale", typeof(FP), 1)]
public partial class ProjectFieldInfo : BehaviorInfo
{
    public string name { get; set; }

    protected override void OnReady()
    {
    }
}
