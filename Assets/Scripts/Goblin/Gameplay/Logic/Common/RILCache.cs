using System;
using System.Collections.Generic;

namespace Goblin.Gameplay.Logic.Common
{
    /// <summary>
    /// 对象池（非并发版本，使用 lock 实现线程安全）
    /// </summary>
    public static class RILCache
    {
        /// <summary>
        /// 同步锁对象
        /// </summary>
        private static readonly object @lock = new();
        /// <summary>
        /// 对象池字典，键为类型，值为该类型的对象队列
        /// </summary>
        private static readonly Dictionary<Type, Queue<object>> pool = new();

        /// <summary>
        /// 从对象池获得一个实例化对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>实例化对象</returns>
        public static T Ensure<T>() where T : new()
        {
            return (T)Ensure(typeof(T));
        }

        /// <summary>
        /// 从对象池获得一个实例化对象
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>实例化对象</returns>
        public static object Ensure(Type type)
        {
            lock (@lock)
            {
                if (pool.TryGetValue(type, out var queue) && queue.Count > 0)
                {
                    return queue.Dequeue();
                }
            }

            return Activator.CreateInstance(type);
        }

        /// <summary>
        /// 将一个实例化对象存入对象池
        /// </summary>
        /// <param name="obj">实例化对象</param>
        public static void Set(object obj)
        {
            if (obj == null) return;

            var type = obj.GetType();
            lock (@lock)
            {
                if (!pool.TryGetValue(type, out var queue))
                {
                    queue = new Queue<object>();
                    pool[type] = queue;
                }

                queue.Enqueue(obj);
            }
        }
    }
}