using System.Collections;
using System.Collections.Generic;
using Goblin.Common;

namespace Goblin.Gameplay.Logic.Common;

/// <summary>
/// 池感知 HashSet，遵循 IGBL 生命周期
/// </summary>
public class GBLHashSet<T> : IGBL, IEnumerable<T>
{
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

	public bool Rmv(T item) => inner.Remove(item);

	public bool Contains(T item) => inner.Contains(item);

	public void Clear() => inner.Clear();

	public HashSet<T>.Enumerator GetEnumerator() => inner.GetEnumerator();

	IEnumerator<T> IEnumerable<T>.GetEnumerator() => inner.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => inner.GetEnumerator();

	public void Reset()
	{
		inner.Clear();
		ObjectCache.Set(inner);
		inner = ObjectCache.Ensure<HashSet<T>>();
	}

	public IGBL Clone()
	{
		var c = ObjectCache.Ensure<GBLHashSet<T>>();
		foreach (var item in inner) c.Add(item);
		return c;
	}
}
