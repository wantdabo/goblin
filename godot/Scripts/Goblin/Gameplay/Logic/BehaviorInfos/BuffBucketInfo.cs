using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.BehaviorInfos
{
    /// <summary>
    /// Buff 桶信息
    /// </summary>
    public class BuffBucketInfo : BehaviorInfo
    {
        /// <summary>
        /// Buff 列表
        /// </summary>
        public List<ulong> buffs { get; set; }
        /// <summary>
        /// Buff 字典, 键为 BuffID, 值为 ActorID
        /// </summary>
        public Dictionary<int, ulong> buffdict { get; set; }
        
        protected override void OnReady()
        {
            buffs = ObjectCache.Ensure<List<ulong>>();
            buffdict = ObjectCache.Ensure<Dictionary<int, ulong>>();
        }

        protected override void OnReset()
        {
            buffs.Clear();
            ObjectCache.Set(buffs);
            
            buffdict.Clear();
            ObjectCache.Set(buffdict);
        }

        protected override BehaviorInfo OnClone()
        {
            var clone = ObjectCache.Ensure<BuffBucketInfo>();
            clone.Ready(actor);
            clone.buffs.AddRange(buffs);
            foreach (var kv in buffdict) clone.buffdict.Add(kv.Key, kv.Value);
            
            return clone;
        }
    }
}