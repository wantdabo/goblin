using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.BehaviorInfos
{
    public class BuffBucketInfo : BehaviorInfo
    {
        private List<ulong> buffs { get; set; }
        
        protected override void OnReady()
        {
            buffs = ObjectCache.Ensure<List<ulong>>();
        }

        protected override void OnReset()
        {
            buffs.Clear();
            ObjectCache.Set(buffs);
        }

        protected override BehaviorInfo OnClone()
        {
            var clone = ObjectCache.Ensure<BuffBucketInfo>();
            clone.Ready(actor);
            clone.buffs.AddRange(buffs);
            
            return clone;
        }
    }
}