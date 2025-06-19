using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.BehaviorInfos.Flows.Common
{
    /// <summary>
    /// Flow 碰撞信息基类
    /// </summary>
    public abstract class FlowCollisionInfo : BehaviorInfo
    {
        /// <summary>
        /// 碰撞记录
        /// </summary>
        public Dictionary<(uint pipeline, uint index), Dictionary<ulong, uint>> records { get; set; }
        /// <summary>
        /// 碰撞的 ActorID 列表
        /// </summary>
        public Queue<(ulong actor, (uint pipeline, uint index) identity)> targets { get; set; }
        
        protected override void OnReady()
        {
            records = ObjectCache.Ensure<Dictionary<(uint pipeline, uint index), Dictionary<ulong, uint>>>();
            targets = ObjectCache.Ensure<Queue<(ulong actor, (uint pipeline, uint index) identity)>>();
        }

        protected override void OnReset()
        {
            foreach (var kv in records)
            {
                kv.Value.Clear();
                ObjectCache.Set(kv.Value);
            }
            records.Clear();
            ObjectCache.Set(records);
            
            targets.Clear();
            ObjectCache.Set(targets);
        }

        protected override BehaviorInfo OnClone()
        {
            var clone = ObjectCache.Ensure<FlowCollisionHurtInfo>();
            clone.Ready(actor);
            clone.records = ObjectCache.Ensure<Dictionary<(uint pipeline, uint index), Dictionary<ulong, uint>>>();
            foreach (var kv in records)
            {
                var record = ObjectCache.Ensure<Dictionary<ulong, uint>>();
                foreach (var kv2 in kv.Value)
                {
                    record.Add(kv2.Key, kv2.Value);
                }
                clone.records.Add(kv.Key, record);
            }
            
            foreach (var id in targets)
            {
                clone.targets.Enqueue(id);
            }
            
            return clone;
        }
    }
}