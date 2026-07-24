using System.Collections.Generic;
using Goblin.Common;
using Goblin.Gameplay.Logic.Common;
using Goblin.Logic.Standalone.TestFixtures;

namespace Goblin.Logic.Tests;

/// <summary>
/// T1.5 GBLList 基础测试（无追踪）
/// </summary>
[Collection("GBL")]
public class GBLListTests
{
    // ============================================================
    // 基本操作
    // ============================================================

    [Fact]
    public void AddAndRetrieve_Success()
    {
        var list = new GBLList<int>();
        list.Add(10);
        list.Add(20);

        Assert.Equal(2, list.Count);
        Assert.Equal(10, list[0]);
        Assert.Equal(20, list[1]);
    }

    [Fact]
    public void RemoveAt_ElementShifted()
    {
        var list = new GBLList<int>();
        list.Add(10);
        list.Add(20);
        list.Add(30);

        list.RemoveAt(1);

        Assert.Equal(2, list.Count);
        Assert.Equal(10, list[0]);
        Assert.Equal(30, list[1]);
    }

    [Fact]
    public void Insert_AtPosition()
    {
        var list = new GBLList<int>();
        list.Add(10);
        list.Add(30);

        list.Insert(1, 20);

        Assert.Equal(3, list.Count);
        Assert.Equal(20, list[1]);
    }

    [Fact]
    public void Indexer_ModifiesElement()
    {
        var list = new GBLList<int>();
        list.Add(10);

        list[0] = 999;

        Assert.Equal(999, list[0]);
    }

    [Fact]
    public void Remove_ByValue()
    {
        var list = new GBLList<int>();
        list.Add(10);
        list.Add(20);

        var removed = list.Remove(10);

        Assert.True(removed);
        Assert.Single(list);
        Assert.Equal(20, list[0]);
    }

    [Fact]
    public void Remove_MissingValue_ReturnsFalse()
    {
        var list = new GBLList<int>();
        list.Add(10);

        var removed = list.Remove(999);

        Assert.False(removed);
    }

    [Fact]
    public void Clear_EmptiesList()
    {
        var list = new GBLList<int>();
        list.Add(10);
        list.Add(20);

        list.Clear();

        Assert.Equal(0, list.Count);
    }

    // ============================================================
    // 池管理
    // ============================================================

    [Fact]
    public void Reset_ReturnsIGBLElementsToPool()
    {
        var list = new GBLList<PooledItem>();
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
        var list = new GBLList<PooledItem>();
        var item = new PooledItem { x = 42, y = 99 };
        list.Add(item);

        list.RemoveAt(0);

        Assert.Equal(0, list.Count);
        var reused = ObjectCache.Ensure<PooledItem>();
        Assert.Equal(0, reused.x);
        Assert.Equal(0, reused.y);
        ObjectCache.Set(reused);
    }

    [Fact]
    public void Clear_ReturnsAllIGBLElementsToPool()
    {
        var list = new GBLList<PooledItem>();
        var item1 = new PooledItem { x = 1, y = 2 };
        var item2 = new PooledItem { x = 3, y = 4 };
        list.Add(item1);
        list.Add(item2);

        list.Clear();

        Assert.Equal(0, list.Count);
        var r1 = ObjectCache.Ensure<PooledItem>();
        var r2 = ObjectCache.Ensure<PooledItem>();
        Assert.Equal(0, r1.x);
        Assert.Equal(0, r2.x);
        ObjectCache.Set(r1);
        ObjectCache.Set(r2);
    }

    // ============================================================
    // Clone
    // ============================================================

    [Fact]
    public void Clone_CopiesData()
    {
        var list = new GBLList<int>();
        list.Add(10);
        list.Add(20);

        var clone = (GBLList<int>)list.Clone();

        Assert.Equal(2, clone.Count);
        Assert.Equal(10, clone[0]);
        Assert.Equal(20, clone[1]);
    }

    [Fact]
    public void Clone_IndependentCopy()
    {
        var list = new GBLList<int>();
        list.Add(10);

        var clone = (GBLList<int>)list.Clone();
        clone[0] = 999;

        Assert.Equal(10, list[0]);
    }

    // ============================================================
    // IGBL 接口
    // ============================================================

    [Fact]
    public void IsIGBL()
    {
        var list = new GBLList<int>();
        Assert.True(list is IGBL);
    }

    [Fact]
    public void IGBL_Clone_ReturnsCorrectType()
    {
        var list = new GBLList<int>();
        var clone = ((IGBL)list).Clone();
        Assert.NotNull(clone);
        Assert.IsType<GBLList<int>>(clone);
    }
}
