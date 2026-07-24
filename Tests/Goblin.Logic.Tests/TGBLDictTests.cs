using System.Collections.Generic;
using Goblin.Common;
using Goblin.Gameplay.Logic.Common;
using Goblin.Logic.Standalone.TestFixtures;

namespace Goblin.Logic.Tests;

/// <summary>
/// T1.5 TGBLDict 脏追踪测试
/// </summary>
[Collection("GBL")]
public class TGBLDictTests
{
    // ============================================================
    // 写入即记账
    // ============================================================

    [Fact]
    public void Add_TracksAddedKey()
    {
        var dict = new TGBLDict<int, uint>();
        dict[1] = 100u;
        dict[2] = 200u;

        var diff = dict.CollectDiff();
        Assert.Equal(2, diff.addedkeys.Count);
        Assert.Contains(1, diff.addedkeys);
        Assert.Contains(2, diff.addedkeys);
        Assert.Empty(diff.removedkeys);
        Assert.Empty(diff.changedkeys);
    }

    [Fact]
    public void Remove_TracksRemovedKey()
    {
        var dict = new TGBLDict<int, uint>();
        dict[1] = 100u;
        dict.CollectDiff();

        dict.Remove(1);

        var diff = dict.CollectDiff();
        Assert.Empty(diff.addedkeys);
        Assert.Single(diff.removedkeys);
        Assert.Contains(1, diff.removedkeys);
    }

    [Fact]
    public void Modify_TracksChangedKey()
    {
        var dict = new TGBLDict<int, uint>();
        dict[1] = 100u;
        dict.CollectDiff();

        dict[1] = 999u;

        var diff = dict.CollectDiff();
        Assert.Empty(diff.addedkeys);
        Assert.Empty(diff.removedkeys);
        Assert.Single(diff.changedkeys);
        Assert.Contains(1, diff.changedkeys);
    }

    [Fact]
    public void ModifySameValue_NotTracked()
    {
        var dict = new TGBLDict<int, uint>();
        dict[1] = 100u;
        dict.CollectDiff();

        dict[1] = 100u;

        var diff = dict.CollectDiff();
        Assert.True(diff.isempty);
    }

    // ============================================================
    // 增删抵消
    // ============================================================

    [Fact]
    public void AddThenRemove_SameKey_CancelsOut()
    {
        var dict = new TGBLDict<int, uint>();
        dict[1] = 100u;
        dict.Remove(1);

        var diff = dict.CollectDiff();
        Assert.True(diff.isempty);
    }

    [Fact]
    public void RemoveThenAdd_SameKey_CancelsOut()
    {
        var dict = new TGBLDict<int, uint>();
        dict[1] = 100u;
        dict.CollectDiff();

        dict.Remove(1);
        dict[1] = 200u;

        var diff = dict.CollectDiff();
        Assert.Empty(diff.addedkeys);
        Assert.Empty(diff.removedkeys);
        Assert.Single(diff.changedkeys);
    }

    // ============================================================
    // Reset
    // ============================================================

    [Fact]
    public void Reset_ClearsDataAndTracking()
    {
        var dict = new TGBLDict<int, ulong>();
        dict[1] = 100ul;
        dict[2] = 200ul;

        dict.Reset();

        Assert.Equal(0, dict.Count);

        var diff = dict.CollectDiff();
        Assert.True(diff.isempty);
    }

    [Fact]
    public void Reset_ReturnsIGBLElementsToPool()
    {
        var dict = new TGBLDict<int, PooledItem>();
        var item = new PooledItem { x = 5, y = 10 };
        dict[1] = item;

        dict.Reset();

        Assert.Equal(0, dict.Count);
        var reused = ObjectCache.Ensure<PooledItem>();
        Assert.Equal(0, reused.x);
        Assert.Equal(0, reused.y);
        ObjectCache.Set(reused);
    }

    // ============================================================
    // Clone
    // ============================================================

    [Fact]
    public void Clone_DeepCopiesData_NotTracking()
    {
        var dict = new TGBLDict<int, uint>();
        dict[1] = 100u;
        dict[2] = 200u;
        dict[2] = 999u;

        var clone = (TGBLDict<int, uint>)dict.Clone();

        Assert.Equal(2, clone.Count);
        Assert.Equal(100u, clone[1]);
        Assert.Equal(999u, clone[2]);

        // 追踪状态不会拷贝
        var cloneDiff = clone.CollectDiff();
        Assert.True(cloneDiff.isempty);

        // 原对象追踪状态不受影响
        var origDiff = dict.CollectDiff();
        Assert.False(origDiff.isempty);
    }

    [Fact]
    public void Clone_IndependentCopy()
    {
        var dict = new TGBLDict<int, uint>();
        dict[1] = 100u;

        var clone = (TGBLDict<int, uint>)dict.Clone();
        clone[1] = 999u;

        Assert.Equal(100u, dict[1]);
    }

    // ============================================================
    // IGBL 接口
    // ============================================================

    [Fact]
    public void IsIGBL()
    {
        var dict = new TGBLDict<int, uint>();
        Assert.True(dict is IGBL);
    }

    [Fact]
    public void IGBL_Clone_ReturnsCorrectType()
    {
        var dict = new TGBLDict<int, uint>();

        var clone = ((IGBL)dict).Clone();

        Assert.NotNull(clone);
        Assert.IsType<TGBLDict<int, uint>>(clone);
    }

    [Fact]
    public void CollectDiff_ThenEmpty_IsEmpty()
    {
        var dict = new TGBLDict<int, uint>();
        dict[1] = 100u;
        dict.CollectDiff();

        var diff2 = dict.CollectDiff();
        Assert.True(diff2.isempty);
    }
}
