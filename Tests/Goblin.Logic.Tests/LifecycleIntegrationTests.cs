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
    [Fact]
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
    [Fact]
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
    [Fact]
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
    /// ProjectFieldInfo 脏标记验证
    /// SG 生成 position/scale 属性后，setter 自动写 projectdirtymask
    /// </summary>
    [Fact]
    public void ProjectFieldInfo_Reset_ClearsDirtyMask()
    {
        var info = new ProjectFieldInfo();
        info.Ready(1);

        // 初始脏标记为 0
        Assert.Equal(0ul, info.projectdirtymask);

        // 设置 position（index 0）→ mask 位 0 置 1
        info.position = new FPVector3(1, 2, 3);
        Assert.Equal(1ul << 0, info.projectdirtymask);

        // 设置 scale（index 1）→ mask 位 1 置 1
        info.scale = new FP(2);
        Assert.Equal((1ul << 0) | (1ul << 1), info.projectdirtymask);

        // TakeProjectValues 只取脏字段
        var values = info.TakeProjectValues(info.projectdirtymask);
        Assert.Equal(2, values.Length);

        // ClearProjectDirty 清零
        info.ClearProjectDirty();
        Assert.Equal(0ul, info.projectdirtymask);
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
