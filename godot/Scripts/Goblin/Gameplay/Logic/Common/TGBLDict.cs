using System;
using System.Collections.Generic;
using Goblin.Common;

namespace Goblin.Gameplay.Logic.Common;

/// <summary>
/// 脏追踪字典 — 继承 GBLDict，增加增删改差量收集
/// </summary>
public class TGBLDict<K, V> : GBLDict<K, V>
{
    private HashSet<K> addedkeys { get; set; } = new();
    private HashSet<K> removedkeys { get; set; } = new();
    private HashSet<K> changedkeys { get; set; } = new();

    private List<K> addbuffer { get; set; } = new();
    private List<K> rmvbuffer { get; set; } = new();
    private List<K> chgbuffer { get; set; } = new();

    public override V this[K key]
    {
        get => base[key];
        set
        {
            if (data.TryGetValue(key, out var old))
            {
                if (EqualityComparer<V>.Default.Equals(old, value)) return;
                data[key] = value;
                if (isvalueigbl)
                {
                    // 回收被覆写的旧 IGBL 值
                    var oldigbl = (IGBL)(object)old;
                    oldigbl.Reset();
                    ObjectCache.Set(oldigbl);
                }
                if (false == addedkeys.Contains(key))
                    changedkeys.Add(key);
            }
            else
            {
                data[key] = value;
                order.Add(key);
                if (removedkeys.Contains(key))
                {
                    removedkeys.Remove(key);
                    changedkeys.Add(key);
                }
                else
                {
                    addedkeys.Add(key);
                }
            }
        }
    }

    public override bool Remove(K key)
    {
        if (false == base.Remove(key)) return false;
        if (addedkeys.Contains(key))
        {
            addedkeys.Remove(key);
        }
        else
        {
            if (changedkeys.Contains(key))
                changedkeys.Remove(key);
            removedkeys.Add(key);
        }
        return true;
    }

    public override void Add(K key, V value)
    {
        data.Add(key, value);
        order.Add(key);
        if (removedkeys.Contains(key))
        {
            removedkeys.Remove(key);
            changedkeys.Add(key);
        }
        else
        {
            addedkeys.Add(key);
        }
    }

    public override void Clear()
    {
        // 先收集移除差量（必须在 base.Clear 清空 data 之前）
        foreach (var kv in data)
        {
            if (false == addedkeys.Contains(kv.Key))
                removedkeys.Add(kv.Key);
        }
        base.Clear();
        addedkeys.Clear();
        changedkeys.Clear();
    }

    public override void Reset()
    {
        base.Reset();
        addedkeys.Clear();
        removedkeys.Clear();
        changedkeys.Clear();
        addbuffer.Clear();
        rmvbuffer.Clear();
        chgbuffer.Clear();
    }

    /// <summary>
    /// 收集本轮差量并重置追踪状态
    /// 返回的 List 引用由内部缓冲区持有，调用方须在下一次 CollectDiff 前消费完毕
    /// </summary>
    public DiffResult<K> CollectDiff()
    {
        addbuffer.Clear();
        addbuffer.AddRange(addedkeys);
        rmvbuffer.Clear();
        rmvbuffer.AddRange(removedkeys);
        chgbuffer.Clear();
        chgbuffer.AddRange(changedkeys);
        addedkeys.Clear();
        removedkeys.Clear();
        changedkeys.Clear();
        return new DiffResult<K>
        {
            addedkeys = addbuffer,
            removedkeys = rmvbuffer,
            changedkeys = chgbuffer
        };
    }

    public override IGBL Clone()
    {
        var c = ObjectCache.Ensure<TGBLDict<K, V>>();
        foreach (var key in order)
        {
            var val = data[key];
            if (val is IGBL igblval)
            {
                c.data[key] = (V)(object)igblval.Clone();
            }
            else
            {
                c.data[key] = val;
            }
            c.order.Add(key);
        }
        return c;
    }
}

/// <summary>
/// 字典差量结果
/// </summary>
public struct DiffResult<K>
{
    public List<K> addedkeys { get; set; }
    public List<K> removedkeys { get; set; }
    public List<K> changedkeys { get; set; }
    public bool isempty => addedkeys.Count == 0 && removedkeys.Count == 0 && changedkeys.Count == 0;
}
