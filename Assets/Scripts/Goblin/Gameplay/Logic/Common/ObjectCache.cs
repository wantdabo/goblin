using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.Common
{
    /// <summary>
    /// 对象池
    /// </summary>
    public static class ObjectCache
    {
        /// <summary>
        /// 对象池字典，键为类型，值为该类型的对象池（字典，键为关键字，值为对象队列）
        /// </summary>
        private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, ConcurrentQueue<object>>> pool = new();

        /// <summary>
        /// 从对象池获得一个实例化对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">KEY/关键字</param>
        /// <returns>实例化对象</returns>
        public static T Get<T>(string key = "") where T : new()
        {
            return (T)Get(typeof(T), key);
        }
        
        /// <summary>
        /// 从对象池获得一个实例化对象
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="key">KEY/关键字</param>
        /// <returns>实例化对象</returns>
        public static object Get(Type type, string key = "")
        {
            if (pool.TryGetValue(type, out var dict) && dict.TryGetValue(key, out var queue) && queue.Count > 0)
            {
                if (queue.TryDequeue(out var obj)) return obj;
            }

            return Activator.CreateInstance(type);
        }

        /// <summary>
        /// 将一个实例化对象存入对象池
        /// </summary>
        /// <param name="obj">实例化对象</param>
        /// <param name="key">KEY/关键字</param>
        public static void Set(object obj, string key = "")
        {
            if (null == obj) return;

            var type = obj.GetType();
            if (false == pool.TryGetValue(type, out var dict))
            {
                dict = new();
                pool.TryAdd(type, dict);
            }

            if (false == dict.TryGetValue(key, out var queue))
            {
                queue = new();
                dict.TryAdd(key, queue);
            }

            queue.Enqueue(obj);
        }
    }
}