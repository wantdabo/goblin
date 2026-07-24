using System.Collections.Generic;
using Goblin.Common;
using Goblin.Gameplay.Logic.Common;
using Goblin.Logic.Standalone.TestFixtures;

namespace Goblin.Logic.Tests;

/// <summary>
/// T1.5 TGBLList 脏追踪测试
/// </summary>
[Collection("GBL")]
public class TGBLListTests
{
    // ============================================================
    // 写入即记账
    // ============================================================

    [Fact]
    public void Add_TracksAddedIndex()
    {
        var list = new TGBLList<int>();
        list.Add(10);
        list.Add(20);

        var diff = list.CollectDiff();
        Assert.Equal(2, diff.addedindices.Count);
        Assert.Contains(0, diff.addedindices);
        Assert.Contains(1, diff.addedindices);
        Assert.Empty(diff.removedindices);
    }

    [Fact]
    public void RemoveAt_TracksRemovedIndex()
    {
        var list = new TGBLList<int>();
        list.Add(10);
        list.Add(20);
        list.CollectDiff();

        list.RemoveAt(0);

        var diff = list.CollectDiff();
        Assert.Empty(diff.addedindices);
        Assert.Single(diff.removedindices);
        Assert.Contains(0, diff.removedindices);
    }

    [Fact]
    public void Indexer_TracksAddedIndex()
    {
        var list = new TGBLList<int>();
        list.Add(10);
        list.CollectDiff();

        list[0] = 99;

        var diff = list.CollectDiff();
        Assert.Single(diff.addedindices);
        Assert.Contains(0, diff.addedindices);
    }

    // ============================================================
    // 增删抵消
    // ============================================================

    [Fact]
    public void AddThenRemoveAt_SameIndex_CancelsOut()
    {
        var list = new TGBLList<int>();
        list.Add(10);
        list.RemoveAt(0);

        var diff = list.CollectDiff();
        Assert.True(diff.isempty);
    }

    [Fact]
    public void RemoveThenAdd_SameIndex_CancelsOut()
    {
        var list = new TGBLList<int>();
        list.Add(10);
        list.Add(20);
        list.CollectDiff();

        list.RemoveAt(0);
        list.Insert(0, 30);

        var diff = list.CollectDiff();
        Assert.Single(diff.addedindices);
        Assert.Contains(0, diff.addedindices);
        Assert.Empty(diff.removedindices);
    }

    // ============================================================
    // Reset
    // ============================================================

    [Fact]
    public void Reset_ClearsDataAndTracking()
    {
        var list = new TGBLList<uint>();
        list.Add(1);
        list.Add(2);

        list.Reset();

        Assert.Equal(0, list.Count);
        var diff = list.CollectDiff();
        Assert.True(diff.isempty);
    }

    [Fact]
    public void Reset_ReturnsIGBLElementsToPool()
    {
        var list = new TGBLList<PooledItem>();
        var item = new PooledItem { x = 5, y = 10 };
        list.Add(item);

        list.Reset();

        Assert.Equal(0, list.Count);
        var reused = ObjectCache.Ensure<PooledItem>();
        Assert.Equal(0, reused.x);
        Assert.Equal(0, reused.y);
        ObjectCache.Set(reused);
    }

    [Fact]
    public void RemoveAt_ReturnsIGBLElementToPool()
    {
        var list = new TGBLList<PooledItem>();
        var item = new PooledItem { x = 42, y = 99 };
        list.Add(item);
        list.CollectDiff();

        list.RemoveAt(0);

        Assert.Equal(0, list.Count);
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
        var list = new TGBLList<int>();
        list.Add(10);
        list.Add(20);

        var clone = (TGBLList<int>)list.Clone();

        Assert.Equal(2, clone.Count);
        Assert.Equal(10, clone[0]);
        Assert.Equal(20, clone[1]);

        var cloneDiff = clone.CollectDiff();
        Assert.True(cloneDiff.isempty);

        var origDiff = list.CollectDiff();
        Assert.False(origDiff.isempty);
    }

    [Fact]
    public void Clone_IndependentCopy()
    {
        var list = new TGBLList<int>();
        list.Add(10);

        var clone = (TGBLList<int>)list.Clone();
        clone[0] = 999;

        Assert.Equal(10, list[0]);
    }

    // ============================================================
    // IGBL 接口
    // ============================================================

    [Fact]
    public void IsIGBL()
    {
        var list = new TGBLList<int>();
        Assert.True(list is IGBL);
    }

    [Fact]
    public void IGBL_Clone_ReturnsCorrectType()
    {
        var list = new TGBLList<int>();

        var clone = ((IGBL)list).Clone();

        Assert.NotNull(clone);
        Assert.IsType<TGBLList<int>>(clone);
    }

    [Fact]
    public void CollectDiff_ThenEmpty_IsEmpty()
    {
        var list = new TGBLList<int>();
        list.Add(10);
        list.CollectDiff();

        var diff2 = list.CollectDiff();
        Assert.True(diff2.isempty);
    }
}
