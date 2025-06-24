using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.BehaviorInfos
{
    public class BuffBucketInfo : BehaviorInfo
    {
        public List<ulong> buffs { get; set; }
        public Dictionary<uint, ulong> buffdict { get; set; }
        
        protected override void OnReady()
        {
            buffs = ObjectCache.Ensure<List<ulong>>();
            buffdict = ObjectCache.Ensure<Dictionary<uint, ulong>>();
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