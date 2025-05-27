using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.BehaviorInfos
{
    /// <summary>
    /// 事件订阅派发者信息
    /// </summary>
    public class EventorInfo : BehaviorInfo
    {
        /// <summary>
        /// 事件订阅派发者的增量计数器, 用于生成唯一的事件索引 (用作排序)
        /// </summary>
        public uint increment { get; set; }
        
        /// <summary>
        /// 事件索引字典, 用于存储事件的索引 (用作排序)
        /// </summary>
        public Dictionary<(int, ulong actor), uint> indexes { get; set; }
        
        protected override void OnReady()
        {
            increment = 0;
            indexes = ObjectCache.Ensure<Dictionary<(int, ulong), uint>>();
        }

        protected override void OnReset()
        {
            increment = 0;
            indexes.Clear();
            ObjectCache.Set(indexes);
        }

        protected override BehaviorInfo OnClone()
        {
            var clone = ObjectCache.Ensure<EventorInfo>();
            clone.Ready(actor);
            clone.increment = increment;
            clone.indexes = ObjectCache.Ensure<Dictionary<(int type, ulong actor), uint>>();
            foreach (var kv in indexes) clone.indexes.Add(kv.Key, kv.Value);
            
            return clone;
        }
    }
}