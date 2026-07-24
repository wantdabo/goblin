using System;
using System.Collections;
using System.Collections.Generic;
using Goblin.Common;

namespace Goblin.Gameplay.Logic.Common;

/// <summary>
/// 池感知字典 — 增删元素时自动处理 IGBL 元素的 Reset + 还池
/// 内部维护插入顺序列表，保证跨平台有序遍历
/// 无变更追踪能力，需要追踪用 TGBLDict
/// </summary>
public class GBLDict<K, V> : IEnumerable<KeyValuePair<K, V>>, IGBL
{
    protected static bool isvalueigbl { get; } = typeof(IGBL).IsAssignableFrom(typeof(V));

    protected Dictionary<K, V> data { get; set; } = new();

    /// <summary>
    /// 插入顺序键列表，保证跨平台有序遍历
    /// </summary>
    protected List<K> order { get; set; } = new();

    // ============================================================
    // 属性
    // ============================================================

    public int Count => data.Count;

    public virtual V this[K key]
    {
        get => data[key];
        set
        {
            if (false == data.ContainsKey(key))
                order.Add(key);
            data[key] = value;
        }
    }

    // ============================================================
    // 字典操作
    // ============================================================

    public bool TryGetValue(K key, out V value) => data.TryGetValue(key, out value);

    public bool ContainsKey(K key) => data.ContainsKey(key);

    public virtual bool Remove(K key)
    {
        if (false == data.TryGetValue(key, out var val)) return false;
        if (isvalueigbl)
        {
            var igbl = (IGBL)val;
            igbl.Reset();
            ObjectCache.Set(igbl);
        }
        data.Remove(key);
        order.Remove(key);
        return true;
    }

    public virtual void Add(K key, V value)
    {
        data.Add(key, value);
        order.Add(key);
    }

    public virtual void Clear()
    {
        if (isvalueigbl)
        {
            foreach (var kv in data)
            {
                var igbl = (IGBL)kv.Value;
                igbl.Reset();
                ObjectCache.Set(igbl);
            }
        }
        data.Clear();
        order.Clear();
    }

    // ============================================================
    // IGBL
    // ============================================================

    public virtual void Reset()
    {
        if (isvalueigbl)
        {
            foreach (var kv in data)
            {
                var igbl = (IGBL)kv.Value;
                igbl.Reset();
                ObjectCache.Set(igbl);
            }
        }
        data.Clear();
        order.Clear();
    }

    public virtual IGBL Clone()
    {
        var c = ObjectCache.Ensure<GBLDict<K, V>>();
        foreach (var key in order)
        {
            var val = data[key];
            if (val is IGBL igblval)
                c.Add(key, (V)(object)igblval.Clone());
            else
                c.Add(key, val);
        }
        return c;
    }

    // ============================================================
    // IEnumerable
    // ============================================================

    public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
    {
        foreach (var key in order)
            yield return new KeyValuePair<K, V>(key, data[key]);
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
