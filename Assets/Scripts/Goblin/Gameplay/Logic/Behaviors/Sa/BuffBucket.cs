using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.Prefabs;

namespace Goblin.Gameplay.Logic.Behaviors.Sa
{
    public class BuffBucket : Behavior
    {
        public void RmvBuff(ulong actor, uint buffid)
        {
        }

        public void AddBuff(ulong actor, uint buffid, uint layer, ulong duration, List<uint> pipelines = null)
        {
            if (false == stage.SeekBehaviorInfo(actor, out BuffBucketInfo buffbucket)) buffbucket = stage.AddBehaviorInfo<BuffBucketInfo>(actor);
            if (buffbucket.buffdict.TryGetValue(buffid, out var buff))
            {
                
                return;
            }

            buff = stage.Spawn(new BuffPrefabInfo
            {
                buffid = buffid,
                layer = layer,
                owner = actor,
                pipelines = pipelines
            });
        }
    }
}