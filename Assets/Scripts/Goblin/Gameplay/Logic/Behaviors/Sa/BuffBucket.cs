using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.Prefabs;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Behaviors.Sa
{
    public class BuffBucket : Behavior
    {
        public void RmvBuff(ulong owner, uint buffid)
        {
            if (false == stage.SeekBehaviorInfo(owner, out BuffBucketInfo buffbucket)) return;
            if (false == buffbucket.buffdict.TryGetValue(buffid, out var buff)) return;
            
            buffbucket.buffdict.Remove(buffid);
            buffbucket.buffs.Remove(buff);
            stage.RmvActor(buff);
        }

        public BuffInfo AddBuff(ulong owner, uint buffid, uint layer, FP lifetime)
        {
            if (false == stage.SeekBehaviorInfo(owner, out BuffBucketInfo buffbucket)) buffbucket = stage.AddBehaviorInfo<BuffBucketInfo>(owner);
            if (buffbucket.buffdict.ContainsKey(buffid)) return GetBuff(owner, buffid);

            var buff = stage.Spawn(new BuffPrefabInfo
            {
                buffid = buffid,
                layer = layer,
                lifetime = lifetime,
                owner = owner,
            });
            buffbucket.buffs.Add(buff);
            buffbucket.buffdict.Add(buffid, buff);

            return GetBuff(owner, buffid);
        }

        public BuffInfo GetBuff(ulong owner, uint buffid)
        {
            if (false == stage.SeekBehaviorInfo(owner, out BuffBucketInfo buffbucket)) return default;
            if (false == buffbucket.buffdict.TryGetValue(buffid, out var buff) || false == stage.SeekBehaviorInfo(buff, out BuffInfo buffinfo)) return default;

            return buffinfo;
        }

        public void SetBuff(ulong owner, uint buffid, uint layer, FP lifetime)
        {
            if (false == stage.SeekBehaviorInfo(owner, out BuffBucketInfo buffbucket)) return;
            if (false == buffbucket.buffdict.TryGetValue(buffid, out var buff) || false == stage.SeekBehaviorInfo(buff, out BuffInfo buffinfo)) return;
            buffinfo.layer = layer;
            buffinfo.lifetime = lifetime;
        }

        protected override void OnTick(FP tick)
        {
            base.OnTick(tick);
            if (false == stage.SeekBehaviorInfos(out List<BuffBucketInfo> buffbuckets)) return;
            foreach (var buffbucket in buffbuckets)
            {
                var bufftick = tick;
                if (stage.SeekBehaviorInfo(buffbucket.actor, out TickerInfo ticker)) bufftick *= ticker.timescale;
                var buffs = ObjectCache.Ensure<List<ulong>>();
                buffs.AddRange(buffbucket.buffs);
                foreach (var buff in buffs)
                {
                    if (false == stage.SeekBehaviorInfo(buff, out BuffInfo buffinfo)) continue;
                    buffinfo.lifetime -= bufftick;
                    if (FP.Zero >= buffinfo.lifetime) RmvBuff(buffbucket.actor, buffinfo.buffid);
                }
                buffs.Clear();
                ObjectCache.Set(buffs);
            }
        }
    }
}