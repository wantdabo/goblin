using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Kowtow
{
    /// <summary>
    /// 对象池
    /// </summary>
    public class ObjectPool
    {
        private readonly ConcurrentDictionary<Type, ConcurrentQueue<object>> pool = new();

        /// <summary>
        /// 从对象池获得一个实例化对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>实例化对象</returns>
        public T Get<T>() where T : new()
        {
            if (pool.TryGetValue(typeof(T), out var queue) && queue.Count > 0)
            {
                queue.TryDequeue(out var obj);

                return (T)obj;
            }

            return new T();
        }

        /// <summary>
        /// 将一个实例化对象存入对象池
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="obj">实例化对象</param>
        public void Set<T>(T obj)
        {
            if (null == obj) return;

            if (false == pool.TryGetValue(typeof(T), out var queue))
            {
                queue = new();
                pool.TryAdd(typeof(T), queue);
            }

            if (queue.Contains(obj)) return;

            queue.Enqueue(obj);
        }
    }
}
