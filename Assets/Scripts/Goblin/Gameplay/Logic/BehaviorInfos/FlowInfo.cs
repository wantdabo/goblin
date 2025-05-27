using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.BehaviorInfos
{
    /// <summary>
    /// 管线信息
    /// </summary>
    public class FlowInfo : BehaviorInfo
    {
        /// <summary>
        /// 管线的拥有者
        /// </summary>
        public ulong owner { get; set; }
        /// <summary>
        /// 管线的时间长度, 根据管线 ID 列表中区间结束的最大值来计算得出
        /// </summary>
        public ulong length { get; set; }
        /// <summary>
        /// 管线的时间线
        /// </summary>
        public ulong timeline { get; set; }
        /// <summary>
        /// 管线的经过时间, 满足单帧才能执行, 如果溢出, 以此循环执行
        /// </summary>
        public ulong elapsed { get; set; }
        /// <summary>
        /// 管线的 ID 列表, 用于指向管线数据
        /// </summary>
        public List<uint> pipelines { get; set; }
        /// <summary>
        /// 管线的执行中 ID 集合, 用于触发管线生命周期
        /// </summary>
        public Dictionary<uint, List<uint>> doings { get; set; }
        
        protected override void OnReady()
        {
            owner = 0;
            length = 0;
            timeline = 0;
            elapsed = 0;
            pipelines = ObjectCache.Ensure<List<uint>>();
            doings = ObjectCache.Ensure<Dictionary<uint, List<uint>>>();
        }

        protected override void OnReset()
        {
            owner = 0;
            length = 0;
            timeline = 0;
            elapsed = 0;
            pipelines.Clear();
            ObjectCache.Set(pipelines);
            foreach (var doing in doings)
            {
                doing.Value.Clear();
                ObjectCache.Set(doing.Value);
            }
            doings.Clear();
            ObjectCache.Set(doings);
        }

        protected override BehaviorInfo OnClone()
        {
            var clone = ObjectCache.Ensure<FlowInfo>();
            clone.Ready(actor);
            clone.owner = owner;
            clone.length = length;
            clone.timeline = timeline;
            clone.elapsed = elapsed;
            foreach (var pipeline in pipelines) clone.pipelines.Add(pipeline);
            foreach (var doing in doings)
            {
                var list = ObjectCache.Ensure<List<uint>>();
                list.AddRange(doing.Value);
                clone.doings.Add(doing.Key, list);
            }

            return clone;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 31 + actor.GetHashCode();
            hash = hash * 31 + owner.GetHashCode();
            hash = hash * 31 + length.GetHashCode();
            hash = hash * 31 + timeline.GetHashCode();
            hash = hash * 31 + elapsed.GetHashCode();
            foreach (var pipeline in pipelines) hash = hash * 31 + pipeline.GetHashCode();
            foreach (var doing in doings)
            {
                foreach (var id in doing.Value) hash = hash * 31 + id.GetHashCode();
            }
            
            return hash;
        }
    }
}