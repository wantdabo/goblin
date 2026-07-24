using System;
using System.Collections.Generic;
using Goblin.Common;

namespace Goblin.Gameplay.Logic.Common;

/// <summary>
/// 脏追踪列表 — 继承 GBLList，增加增删改差量收集
/// </summary>
public class TGBLList<T> : GBLList<T>
{
    private HashSet<int> addedindices { get; set; } = new();
    private HashSet<int> removedindices { get; set; } = new();

    private List<int> addbuffer { get; set; } = new();
    private List<int> rmvbuffer { get; set; } = new();

    public override T this[int index]
    {
        get => base[index];
        set
        {
            if (EqualityComparer<T>.Default.Equals(data[index], value)) return;
            data[index] = value;
            addedindices.Add(index);
        }
    }

    public override void Add(T item)
    {
        var idx = data.Count;
        base.Add(item);
        if (removedindices.Contains(idx))
        {
            removedindices.Remove(idx);
            addedindices.Add(idx);
        }
        else
        {
            addedindices.Add(idx);
        }
    }

    public override void Insert(int index, T item)
    {
        base.Insert(index, item);
        if (removedindices.Contains(index))
        {
            removedindices.Remove(index);
            addedindices.Add(index);
        }
        else
        {
            addedindices.Add(index);
        }
    }

    public override void RemoveAt(int index)
    {
        base.RemoveAt(index);
        if (addedindices.Contains(index))
        {
            addedindices.Remove(index);
        }
        else
        {
            removedindices.Add(index);
        }
    }

    public override void Clear()
    {
        foreach (var item in data)
        {
            if (iselementigbl)
            {
                var igbl = (IGBL)item;
                igbl.Reset();
                ObjectCache.Set(igbl);
            }
        }
        data.Clear();
        addedindices.Clear();
        removedindices.Clear();
    }

    public override void Reset()
    {
        base.Reset();
        addedindices.Clear();
        removedindices.Clear();
        addbuffer.Clear();
        rmvbuffer.Clear();
    }

    /// <summary>
    /// 收集本轮差量并重置追踪状态
    /// 返回的 List 引用由内部缓冲区持有，调用方须在下一次 CollectDiff 前消费完毕
    /// </summary>
    public ListDiffResult CollectDiff()
    {
        addbuffer.Clear();
        addbuffer.AddRange(addedindices);
        rmvbuffer.Clear();
        rmvbuffer.AddRange(removedindices);
        addedindices.Clear();
        removedindices.Clear();
        return new ListDiffResult
        {
            addedindices = addbuffer,
            removedindices = rmvbuffer
        };
    }

    public override IGBL Clone()
    {
        var c = ObjectCache.Ensure<TGBLList<T>>();
        foreach (var item in data)
        {
            if (item is IGBL igblelem)
                c.data.Add((T)(object)igblelem.Clone());
            else
                c.data.Add(item);
        }
        return c;
    }
}

/// <summary>
/// 列表差量结果
/// </summary>
public struct ListDiffResult
{
    public List<int> addedindices { get; set; }
    public List<int> removedindices { get; set; }
    public bool isempty => addedindices.Count == 0 && removedindices.Count == 0;
}
