using System.Collections.Generic;
using Goblin.Common;
using Goblin.Gameplay.Logic.Common;
using Goblin.Logic.Standalone.TestFixtures;

namespace Goblin.Logic.Tests;

/// <summary>
/// T1.5 GBLDict 基础测试（无追踪）
/// </summary>
[Collection("GBL")]
public class GBLDictTests
{
    // ============================================================
    // 基本操作
    // ============================================================

    [Fact]
    public void AddAndRetrieve_Success()
    {
        var dict = new GBLDict<int, uint>();
        dict[1] = 100u;
        dict[2] = 200u;

        Assert.Equal(2, dict.Count);
        Assert.Equal(100u, dict[1]);
        Assert.Equal(200u, dict[2]);
    }

    [Fact]
    public void Remove_KeyNoLongerExists()
    {
        var dict = new GBLDict<int, uint>();
        dict[1] = 100u;

        var removed = dict.Remove(1);

        Assert.True(removed);
        Assert.Equal(0, dict.Count);
        Assert.False(dict.ContainsKey(1));
    }

    [Fact]
    public void Remove_MissingKey_ReturnsFalse()
    {
        var dict = new GBLDict<int, uint>();

        var removed = dict.Remove(999);

        Assert.False(removed);
    }

    [Fact]
    public void Modify_ValueUpdated()
    {
        var dict = new GBLDict<int, uint>();
        dict[1] = 100u;

        dict[1] = 999u;

        Assert.Equal(999u, dict[1]);
    }

    [Fact]
    public void Clear_EmptiesDictionary()
    {
        var dict = new GBLDict<int, uint>();
        dict[1] = 100u;
        dict[2] = 200u;

        dict.Clear();

        Assert.Equal(0, dict.Count);
    }

    // ============================================================
    // 池管理
    // ============================================================

    [Fact]
    public void Reset_ReturnsIGBLElementsToPool()
    {
        var dict = new GBLDict<int, PooledItem>();
        var item = new PooledItem { x = 5, y = 10 };
        dict[1] = item;

        dict.Reset();

        Assert.Equal(0, dict.Count);
        var reused = ObjectCache.Ensure<PooledItem>();
        Assert.Equal(0, reused.x);
        Assert.Equal(0, reused.y);
        ObjectCache.Set(reused);
    }

    [Fact]
    public void Remove_ReturnsIGBLElementToPool()
    {
        var dict = new GBLDict<int, PooledItem>();
        var item = new PooledItem { x = 42, y = 99 };
        dict[1] = item;

        dict.Remove(1);

        Assert.Equal(0, dict.Count);
        var reused = ObjectCache.Ensure<PooledItem>();
        Assert.Equal(0, reused.x);
        Assert.Equal(0, reused.y);
        ObjectCache.Set(reused);
    }

    [Fact]
    public void Clear_ReturnsAllIGBLElementsToPool()
    {
        var dict = new GBLDict<int, PooledItem>();
        var item1 = new PooledItem { x = 1, y = 2 };
        var item2 = new PooledItem { x = 3, y = 4 };
        dict[1] = item1;
        dict[2] = item2;

        dict.Clear();

        Assert.Equal(0, dict.Count);
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
        var dict = new GBLDict<int, uint>();
        dict[1] = 100u;
        dict[2] = 200u;

        var clone = (GBLDict<int, uint>)dict.Clone();

        Assert.Equal(2, clone.Count);
        Assert.Equal(100u, clone[1]);
        Assert.Equal(200u, clone[2]);
    }

    [Fact]
    public void Clone_IndependentCopy()
    {
        var dict = new GBLDict<int, uint>();
        dict[1] = 100u;

        var clone = (GBLDict<int, uint>)dict.Clone();
        clone[1] = 999u;

        Assert.Equal(100u, dict[1]);
    }

    // ============================================================
    // IGBL 接口
    // ============================================================

    [Fact]
    public void IsIGBL()
    {
        var dict = new GBLDict<int, uint>();
        Assert.True(dict is IGBL);
    }

    [Fact]
    public void IGBL_Clone_ReturnsCorrectType()
    {
        var dict = new GBLDict<int, uint>();
        var clone = ((IGBL)dict).Clone();
        Assert.NotNull(clone);
        Assert.IsType<GBLDict<int, uint>>(clone);
    }
}
