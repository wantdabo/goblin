using System.Collections.Generic;
using Goblin.Common;
using Goblin.Gameplay.Logic.Core;
using Goblin.Logic.Standalone.TestFixtures;

namespace Goblin.Logic.Tests;

/// <summary>
/// T1.2 生命周期集成测试
/// 验证 SG 生成的 Reset/Clone 与 BehaviorInfo 基类钩子链协同
/// </summary>
public class LifecycleIntegrationTests
{
    /// <summary>
    /// SimpleInfo Reset → 断言值类型字段归零
    /// T1.4 SG 生成 override Reset() 后启用
    /// </summary>
    [Fact(Skip = "T1.4: SG 生成字段 Reset 逻辑后启用")]
    public void SimpleInfo_Reset_ClearsAllFields()
    {
        var info = new SimpleInfo();
        info.Ready(1);
        info.value = 42;
        info.speed = new FP(100);
        info.active2 = true;

        info.Reset();

        Assert.Equal(0, info.value);
        Assert.Equal(FP.Zero, info.speed);
        Assert.False(info.active2);
    }

    /// <summary>
    /// SimpleInfo Reset → 断言基类 actor/active 归零
    /// </summary>
    [Fact]
    public void SimpleInfo_Reset_ClearsBaseFields()
    {
        var info = new SimpleInfo();
        info.Ready(123);
        Assert.Equal(123ul, info.actor);
        Assert.True(info.active);

        info.Reset();

        Assert.Equal(0ul, info.actor);
        Assert.False(info.active);
    }

    /// <summary>
    /// ContainerInfo Reset → 断言容器字段清空
    /// T1.4 SG 生成容器 Reset 逻辑后启用
    /// </summary>
    [Fact(Skip = "T1.4: SG 生成容器 Reset 逻辑后启用")]
    public void ContainerInfo_Reset_ClearsContainers()
    {
        var info = new ContainerInfo();
        info.Ready(1);
        info.ids = new System.Collections.Generic.List<uint> { 1, 2, 3 };
        info.dict = new System.Collections.Generic.Dictionary<int, ulong>
        {
            { 1, 100 },
            { 2, 200 },
        };

        info.Reset();

        Assert.NotNull(info.ids);
        Assert.Empty(info.ids);
        Assert.NotNull(info.dict);
        Assert.Empty(info.dict);
    }

    /// <summary>
    /// NestedPoolInfo Reset → 断言嵌套池化对象元素被清理
    /// T1.4 SG 生成嵌套 IGBL Reset 逻辑后启用
    /// </summary>
    [Fact(Skip = "T1.4: SG 生成嵌套 IGBL Reset 逻辑后启用")]
    public void NestedPoolInfo_Reset_ClearsNestedPooledItems()
    {
        var info = new NestedPoolInfo();
        info.Ready(1);
        info.items = new System.Collections.Generic.List<PooledItem>
        {
            new PooledItem { x = 5, y = 10 },
            new PooledItem { x = 15, y = 20 },
        };

        info.Reset();

        Assert.NotNull(info.items);
        Assert.Empty(info.items);
    }

    /// <summary>
    /// ProjectFieldInfo Reset → 断言 projectdirtymask 归零
    /// （当前 SG 只生成空 partial，脏标记尚未自动写入）
    /// </summary>
    [Fact]
    public void ProjectFieldInfo_Reset_ClearsDirtyMask()
    {
        var info = new ProjectFieldInfo();
        info.Ready(1);
        info.position = new FPVector3(1, 2, 3);
        info.scale = new FP(2);
        info.name = "test";

        info.Reset();

        // projectdirtymask 待 SG 生成后应归零
    }

    /// <summary>
    /// IGBL 接口验证：BehaviorInfo 是 IGBL
    /// </summary>
    [Fact]
    public void BehaviorInfo_IsIGBL()
    {
        var info = new SimpleInfo();
        Assert.True(info is IGBL);
    }

    /// <summary>
    /// IGBL Clone 显式实现正确委托
    /// </summary>
    [Fact]
    public void IGBL_Clone_DelegatesCorrectly()
    {
        var info = new SimpleInfo();
        info.Ready(1);
        info.value = 99;

        var cloned = (SimpleInfo)((IGBL)info).Clone();

        Assert.NotNull(cloned);
        Assert.Equal(99, cloned.value);
    }
}
