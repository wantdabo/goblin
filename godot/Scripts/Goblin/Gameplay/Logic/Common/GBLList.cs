using System;
using System.Collections;
using System.Collections.Generic;
using Goblin.Common;

namespace Goblin.Gameplay.Logic.Common;

/// <summary>
/// 池感知列表 — 增删元素时自动处理 IGBL 元素的 Reset + 还池
/// 无变更追踪能力，需要追踪用 TGBLList
/// </summary>
public class GBLList<T> : IEnumerable<T>, IGBL
{
    protected static bool iselementigbl { get; } = typeof(IGBL).IsAssignableFrom(typeof(T));

    protected List<T> data { get; set; } = new();

    // ============================================================
    // 属性
    // ============================================================

    public int Count => data.Count;

    public virtual T this[int index]
    {
        get => data[index];
        set => data[index] = value;
    }

    // ============================================================
    // 列表操作
    // ============================================================

    public virtual void Add(T item)
    {
        data.Add(item);
    }

    public virtual void Insert(int index, T item)
    {
        data.Insert(index, item);
    }

    public bool Remove(T item)
    {
        var idx = data.IndexOf(item);
        if (idx < 0) return false;
        RemoveAt(idx);
        return true;
    }

    public bool Contains(T item) => data.Contains(item);

    public int IndexOf(T item) => data.IndexOf(item);

    public void Sort(Comparison<T> comparison) => data.Sort(comparison);

    public virtual void AddRange(IEnumerable<T> items)
    {
        foreach (var item in items)
            Add(item);
    }

    public virtual void RemoveAt(int index)
    {
        if (iselementigbl)
        {
            var elem = (IGBL)data[index];
            elem.Reset();
            ObjectCache.Set(elem);
        }
        data.RemoveAt(index);
    }

    public virtual void Clear()
    {
        if (iselementigbl)
        {
            foreach (var item in data)
            {
                var igbl = (IGBL)item;
                igbl.Reset();
                ObjectCache.Set(igbl);
            }
        }
        data.Clear();
    }

    // ============================================================
    // IGBL
    // ============================================================

    public virtual void Reset()
    {
        if (iselementigbl)
        {
            foreach (var item in data)
            {
                var igbl = (IGBL)item;
                igbl.Reset();
                ObjectCache.Set(igbl);
            }
        }
        data.Clear();
    }

    public virtual IGBL Clone()
    {
        var c = ObjectCache.Ensure<GBLList<T>>();
        foreach (var item in data)
        {
            if (item is IGBL igblelem)
                c.data.Add((T)(object)igblelem.Clone());
            else
                c.data.Add(item);
        }
        return c;
    }

    // ============================================================
    // IEnumerable
    // ============================================================

    public IEnumerator<T> GetEnumerator() => data.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => data.GetEnumerator();
}
