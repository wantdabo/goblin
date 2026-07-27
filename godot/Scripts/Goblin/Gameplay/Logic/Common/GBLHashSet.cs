using System.Collections;
using System.Collections.Generic;
using Goblin.Common;

namespace Goblin.Gameplay.Logic.Common;

/// <summary>
/// 池感知 HashSet，遵循 IGBL 生命周期
/// </summary>
public class GBLHashSet<T> : IGBL, IEnumerable<T>
{
    protected static bool iselementigbl { get; } = typeof(IGBL).IsAssignableFrom(typeof(T));

    /// <summary>
    /// 内部原生 HashSet
    /// </summary>
    private HashSet<T> inner { get; set; }

    /// <summary>
    /// 元素数量
    /// </summary>
    public int count => inner.Count;

    public GBLHashSet()
    {
        inner = ObjectCache.Ensure<HashSet<T>>();
    }

    public GBLHashSet(int capacity)
    {
        inner = ObjectCache.Ensure<HashSet<T>>();
        inner.EnsureCapacity(capacity);
    }

    /// <summary>
    /// 拷贝构造，供 SG Clone 容器值类型使用
    /// </summary>
    public GBLHashSet(IEnumerable<T> collection)
    {
        inner = ObjectCache.Ensure<HashSet<T>>();
        foreach (var item in collection) inner.Add(item);
    }

    public bool Add(T item) => inner.Add(item);

    public bool Rmv(T item)
    {
        if (false == inner.Remove(item)) return false;
        if (iselementigbl)
        {
            var igbl = (IGBL)item;
            igbl.Reset();
            ObjectCache.Set(igbl);
        }
        return true;
    }

    public bool Contains(T item) => inner.Contains(item);

    public void Clear()
    {
        if (iselementigbl)
        {
            foreach (var item in inner)
            {
                var igbl = (IGBL)item;
                igbl.Reset();
                ObjectCache.Set(igbl);
            }
        }
        inner.Clear();
    }

    public HashSet<T>.Enumerator GetEnumerator() => inner.GetEnumerator();

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => inner.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => inner.GetEnumerator();

    public void Reset()
    {
        if (iselementigbl)
        {
            foreach (var item in inner)
            {
                var igbl = (IGBL)item;
                igbl.Reset();
                ObjectCache.Set(igbl);
            }
        }
        inner.Clear();
        ObjectCache.Set(inner);
        inner = ObjectCache.Ensure<HashSet<T>>();
    }

    /// <summary>
    /// 内部元素 Reset + 还池，自身也还池
    /// </summary>
    public void Dispose()
    {
        Reset();
        ObjectCache.Set(this);
    }

    public IGBL Clone()
    {
        var c = ObjectCache.Ensure<GBLHashSet<T>>();
        foreach (var item in inner)
        {
            if (item is IGBL igblelem)
                c.inner.Add((T)(object)igblelem.Clone());
            else
                c.inner.Add(item);
        }
        return c;
    }
}
