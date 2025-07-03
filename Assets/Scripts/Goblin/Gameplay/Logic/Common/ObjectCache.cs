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
        private static readonly Dictionary<Type, Dictionary<string, Queue<object>>> pool = new();
        private static readonly Dictionary<object, Queue<object>> capacityidentitys = new();
        /// <summary>
        /// 容量池字典，键为类型，值为该类型的容量池（字典，键为容量，值为对象队列）
        /// </summary>
        private static readonly Dictionary<Type, Dictionary<int, Queue<object>>> capacitypool = new();

        public static T Ensure<T, TValue>(int capacity) where T : new()
        {
            var type = typeof(T);

            if (false == capacitypool.TryGetValue(type, out var dict))
            {
                dict = new();
                capacitypool.Add(type, dict);
            }

            if (false == dict.TryGetValue(capacity, out var queue))
            {
                queue = new();
                dict.Add(capacity, queue);
            }
            
            if (0 != queue.Count) 
            {
                if (queue.TryDequeue(out var obj)) return (T)obj;
            }

            T result = default;
            if (type == typeof(List<TValue>))
            {
                result = (T)(new List<TValue>(capacity) as object);
            }

            if (type == typeof(HashSet<TValue>))
            {
                result = (T)(new HashSet<TValue>(capacity) as object);
            }

            if (type == typeof(Queue<TValue>))
            {
                result = (T)(new Queue<TValue>(capacity) as object);
            }
            
            if (null == result) throw new InvalidOperationException($"unsupported type: {typeof(T)}");
            
            capacityidentitys.Add(result, queue);

            return result;
        }
        
        public static T Ensure<T, TKey, TValue>(int capacity) where T : new()
        {
            var type = typeof(T);
            if (type == typeof(Dictionary<TKey, TValue>))
            {
                return (T)(new Dictionary<TKey, TValue>(capacity) as object);
            }

            throw new InvalidOperationException($"unsupported type: {typeof(T)}");
        }

        /// <summary>
        /// 从对象池获得一个实例化对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">KEY/关键字</param>
        /// <returns>实例化对象</returns>
        public static T Ensure<T>(string key = "") where T : new()
        {
            var type = typeof(T);
            if (pool.TryGetValue(type, out var dict) && dict.TryGetValue(key, out var queue) && queue.Count > 0)
            {
                if (queue.TryDequeue(out var obj)) return (T)obj;
            }
            
            return new T();
        }
        
        /// <summary>
        /// 从对象池获得一个实例化对象
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="key">KEY/关键字</param>
        /// <returns>实例化对象</returns>
        public static object Ensure(Type type, string key = "")
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

            if (capacityidentitys.TryGetValue(obj, out var queue))
            {
                queue.Enqueue(obj);
                
                return;
            }

            var type = obj.GetType();
            if (false == pool.TryGetValue(type, out var dict))
            {
                dict = new();
                pool.Add(type, dict);
            }

            if (false == dict.TryGetValue(key, out queue))
            {
                queue = new();
                dict.Add(key, queue);
            }
            
            queue.Enqueue(obj);
        }
    }
}