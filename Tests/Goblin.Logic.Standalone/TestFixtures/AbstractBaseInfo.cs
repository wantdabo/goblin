using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Logic.Standalone.TestFixtures;

/// <summary>
/// 模式 5：抽象基类（测试 SG 跳过 Clone 生成 + 父类字段由子类 Clone 包含）
/// </summary>
public abstract partial class AbstractBaseInfo : BehaviorInfo
{
    /// <summary>
    /// 嵌套字典容器
    /// </summary>
    public Dictionary<int, Dictionary<ulong, uint>> records { get; set; }
    /// <summary>
    /// 列表容器
    /// </summary>
    public List<(ulong actor, uint id)> targets { get; set; }

    protected override void OnReady()
    {
        records = ObjectCache.Ensure<Dictionary<int, Dictionary<ulong, uint>>>();
        targets = ObjectCache.Ensure<List<(ulong actor, uint id)>>();
    }
}
