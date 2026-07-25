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
		var clone = new GBLHashSet<T>();
		foreach (var item in inner) clone.Add(item);
		return clone;
	}
}
