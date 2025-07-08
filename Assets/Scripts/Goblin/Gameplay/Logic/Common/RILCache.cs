using System;
using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common.Defines;

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
        /// 容量池字典，键为对象，值为该对象的容量队列
        /// </summary>
        private static readonly Dictionary<object, Queue<object>> capacityidentitys = new();
        /// <summary>
        /// 容量池字典，键为类型，值为该类型的容量池（字典，键为容量，值为对象队列）
        /// </summary>
        private static readonly Dictionary<Type, Dictionary<int, Queue<object>>> capacitypool = new();
        
                /// <summary>
        /// 查询容器对象池对应的队列
        /// </summary>
        /// <param name="capacity">容量</param>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>容器队列</returns>
        private static Queue<object> QueryCapacity<T>(int capacity) where T : new()
        {
            var type = typeof(T);
            if (false == capacitypool.TryGetValue(type, out var dict))
            {
                dict = new(CAPACITY_DEFINE.L0);
                capacitypool.Add(type, dict);
            }
            if (false == dict.TryGetValue(capacity, out var queue))
            {
                queue = new(CAPACITY_DEFINE.L6);
                dict.Add(capacity, queue);
            }

            return queue;
        }

        /// <summary>
        /// 从对象池中获取一个实例化容器，如果不存在则创建一个新的实例
        /// </summary>
        /// <param name="capacity">容量</param>
        /// <typeparam name="T">类型</typeparam>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <returns>容器</returns>
        /// <exception cref="InvalidOperationException">传入的容器类型为不支持类型</exception>
        public static T Ensure<T, TValue>(int capacity) where T : new()
        {
            var queue = QueryCapacity<T>(capacity);
            if (0 != queue.Count) if (queue.TryDequeue(out var obj)) return (T)obj;
            object ins = default;
            var type = typeof(T);
            if (type == typeof(List<TValue>)) ins = new List<TValue>(capacity);
            if (type == typeof(HashSet<TValue>)) ins = new HashSet<TValue>(capacity);
            if (type == typeof(Queue<TValue>)) ins = new Queue<TValue>(capacity);
            if (null == ins) throw new InvalidOperationException($"unsupported type: {typeof(T)}");
            
            capacityidentitys.Add(ins, queue);

            return (T)ins;
        }
        
        /// <summary>
        /// 从对象池中获取一个实例化容器，如果不存在则创建一个新的实例
        /// </summary>
        /// <param name="capacity">容量</param>
        /// <typeparam name="T">类型</typeparam>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <returns>容器</returns>
        /// <exception cref="InvalidOperationException">传入的容器类型为不支持类型</exception>
        public static T Ensure<T, TKey, TValue>(int capacity) where T : new()
        {
            var queue = QueryCapacity<T>(capacity);
            if (0 != queue.Count) if (queue.TryDequeue(out var obj)) return (T)obj;
            object ins = default;
            var type = typeof(T);
            if (type == typeof(Dictionary<TKey, TValue>)) ins = new Dictionary<TKey, TValue>(capacity);
            if (null == ins) throw new InvalidOperationException($"unsupported type: {typeof(T)}");
            capacityidentitys.Add(ins, queue);
            
            return (T)ins;
        }

        /// <summary>
        /// 从对象池获得一个实例化对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>实例化对象</returns>
        public static T Ensure<T>() where T : new()
        {
            var type = typeof(T);
            if (pool.TryGetValue(type, out var queue) && queue.Count > 0)
            {
                if (queue.TryDequeue(out var obj)) return (T)obj;
            }
            
            return new T();
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