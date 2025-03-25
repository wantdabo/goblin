using System;
using System.Collections.Generic;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.Common
{
    public static class ObjectCache
    {
        private static readonly Dictionary<Type, Dictionary<string, Queue<object>>> pool = new();

        /// <summary>
        /// 从对象池获得一个实例化对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">KEY/关键字</param>
        /// <returns>实例化对象</returns>
        public static T Get<T>(string key = "") where T : class, new()
        {
            if (pool.TryGetValue(typeof(T), out var dict) && dict.TryGetValue(key, out var queue) && queue.Count > 0)
            {
                return queue.Dequeue() as T;
            }

            return new T();
        }

        /// <summary>
        /// 将一个实例化对象存入对象池
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="obj">实例化对象</param>
        /// <param name="key">KEY/关键字</param>
        public static void Set<T>(T obj, string key = "")
        {
            if (null == obj) return;

            if (false == pool.TryGetValue(typeof(T), out var dict))
            {
                dict = new();
                pool.Add(typeof(T), dict);
            }

            if (false == dict.TryGetValue(key, out var queue))
            {
                queue = new();
                dict.Add(key, queue);
            }

            if (queue.Contains(obj)) return;

            queue.Enqueue(obj);
        }
    }
}