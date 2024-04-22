using Goblin.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goblin.Common
{
    /// <summary>
    /// 对象池
    /// </summary>
    public class ObjectPool : Comp
    {
        private readonly Dictionary<Type, Dictionary<string, Queue<object>>> pool = new();

        /// <summary>
        /// 从对象池获得一个实例化对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">KEY/关键字</param>
        /// <param name="callback">回调（成功获取才会执行）</param>
        /// <returns>实例化对象</returns>
        public T Get<T>(string key, Action<T> callback = null)
        {
            if (pool.TryGetValue(typeof(T), out var dict) && dict.TryGetValue(key, out var queue) && queue.Count > 0)
            {
                var obj = (T)queue.Dequeue();
                callback?.Invoke(obj);

                return obj;
            }

            return default;
        }

        /// <summary>
        /// 将一个实例化对象存入对象池
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">KEY/关键字</param>
        /// <param name="obj">实例化对象</param>
        /// <param name="callback">回调（成功存入才会执行）</param>
        public void Set<T>(string key, T obj, Action<T> callback = null)
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
            callback?.Invoke(obj);
        }
    }
}
