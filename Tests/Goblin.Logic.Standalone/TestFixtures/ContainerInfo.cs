using System.Collections.Generic;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Logic.Standalone.TestFixtures;

/// <summary>
/// 模式 2：容器字段 — 值类型元素
/// SG 生成 override Reset() → ids.Reset(), dict.Reset(), base.Reset()
/// </summary>
public partial class ContainerInfo : BehaviorInfo
{
    public List<uint> ids { get; set; }
    public Dictionary<int, ulong> dict { get; set; }

    protected override void OnReady()
    {
    }
}
