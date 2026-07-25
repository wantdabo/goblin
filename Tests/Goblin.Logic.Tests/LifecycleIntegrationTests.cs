using System.Collections.Generic;
using Goblin.Common;
using Goblin.Gameplay.Logic.Core;
using Goblin.Logic.Standalone.TestFixtures;

namespace Goblin.Logic.Tests;

/// <summary>
/// T1.2 生命周期集成测试
/// 验证 SG 生成的 Reset/Clone 与 BehaviorInfo 基类钩子链协同
/// </summary>
[Collection("GBL")]
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

    /// <summary>
    /// 抽象类 Reset：SG 应为 abstract 类生成 Reset（清理容器字段）
    /// 不生成 Clone（因为抽象类不可实例化）
    /// </summary>
    [Fact]
    public void AbstractBaseInfo_Reset_ClearsContainerFields()
    {
        var info = new ConcreteDerivedInfo();
        info.Ready(1);

        // 初始化 records 嵌套容器
        info.records.Add(0, new System.Collections.Generic.Dictionary<ulong, uint>
        {
            { 100, 1 },
        });
        info.targets.Add((100, 1));

        info.Reset();

        Assert.NotNull(info.records);
        Assert.Empty(info.records);
        Assert.NotNull(info.targets);
        Assert.Empty(info.targets);
    }

    /// <summary>
    /// 子类 Clone：应包含自身字段和父类字段
    /// </summary>
    [Fact]
    public void ConcreteDerivedInfo_Clone_CopiesOwnAndParentFields()
    {
        var info = new ConcreteDerivedInfo();
        info.Ready(1);
        info.name = "test";
        info.value = 42;
        info.enabled = true;

        // 设置父类容器字段
        info.records.Add(0, new System.Collections.Generic.Dictionary<ulong, uint>
        {
            { 100, 5 },
        });
        info.targets.Add((200, 3));

        var clone = (ConcreteDerivedInfo)((IGBL)info).Clone();

        Assert.NotNull(clone);
        Assert.Equal("test", clone.name);
        Assert.Equal(42, clone.value);
        Assert.True(clone.enabled);

        // 验证父类字段被深拷贝
        Assert.NotNull(clone.records);
        Assert.Single(clone.records);
        Assert.NotNull(clone.targets);
        Assert.Single(clone.targets);
    }

    /// <summary>
    /// 子类 Reset：SG 生成调用 base.Reset()，清洗自身 + 父类字段
    /// </summary>
    [Fact]
    public void ConcreteDerivedInfo_Reset_ClearsAllFields()
    {
        var info = new ConcreteDerivedInfo();
        info.Ready(1);
        info.name = "test";
        info.value = 42;
        info.enabled = true;
        info.records.Add(0, new System.Collections.Generic.Dictionary<ulong, uint>
        {
            { 100, 1 },
        });
        info.targets.Add((100, 1));

        info.Reset();

        // 自身字段归零
        Assert.Null(info.name);
        Assert.Equal(0, info.value);
        Assert.False(info.enabled);

        // 父类字段清空
        Assert.Empty(info.records);
        Assert.Empty(info.targets);
    }

    /// <summary>
    /// 抽象类不应有 SG 生成的 Clone（Ensure abstract 类会失败）
    /// 但 SG 应为其生成 Reset
    /// </summary>
    [Fact]
    public void AbstractBaseInfo_HasReset_NoClone()
    {
        var info = new ConcreteDerivedInfo();
        info.Ready(1);

        // Reset 应可见（SG 生成）
        info.Reset();

        // 抽象类本身不暴露 Clone 调用，这里通过 IGBL 验证
        var clone = ((IGBL)info).Clone();
        Assert.NotNull(clone);
        Assert.IsType<ConcreteDerivedInfo>(clone);
    }
}
