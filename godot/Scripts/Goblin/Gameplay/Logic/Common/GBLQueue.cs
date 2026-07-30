using System.Collections;
using System.Collections.Generic;
using Goblin.Common;

namespace Goblin.Gameplay.Logic.Common;

/// <summary>
/// 池感知队列 — 遵循 IGBL 生命周期
/// </summary>
public class GBLQueue<T> : IGBL, IEnumerable<T>
{
    protected static bool iselementigbl { get; } = typeof(IGBL).IsAssignableFrom(typeof(T));

    /// <summary>
    /// 内部原生队列
    /// </summary>
    private Queue<T> inner { get; set; }

    /// <summary>
    /// 元素数量
    /// </summary>
    public int count => inner.Count;

    public GBLQueue()
    {
        inner = new();
    }

    public GBLQueue(int capacity)
    {
        inner = new(capacity);
    }

    /// <summary>
    /// 入队
    /// </summary>
    public void Enqueue(T item)
    {
        inner.Enqueue(item);
    }

    /// <summary>
    /// 出队 — 抛出异常若队列为空
    /// 不自动 Reset，由调用方消费后手动管理生命周期
    /// </summary>
    public T Dequeue()
    {
        return inner.Dequeue();
    }

    /// <summary>
    /// 尝试出队
    /// 不自动 Reset，由调用方消费后手动管理生命周期
    /// </summary>
    public bool TryDequeue(out T result)
    {
        return inner.TryDequeue(out result);
    }

    /// <summary>
    /// 查看队首 — 不出队
    /// </summary>
    public T Peek() => inner.Peek();

    /// <summary>
    /// 尝试查看队首 — 不出队
    /// </summary>
    public bool TryPeek(out T result) => inner.TryPeek(out result);

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

    public T[] ToArray() => inner.ToArray();

    // ============================================================
    // IGBL
    // ============================================================

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
        var c = ObjectCache.Ensure<GBLQueue<T>>();
        foreach (var item in inner)
        {
            if (item is IGBL igblelem)
                c.inner.Enqueue((T)(object)igblelem.Clone());
            else
                c.inner.Enqueue(item);
        }
        return c;
    }

    // ============================================================
    // IEnumerable
    // ============================================================

    public Queue<T>.Enumerator GetEnumerator() => inner.GetEnumerator();
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => inner.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => inner.GetEnumerator();
}
