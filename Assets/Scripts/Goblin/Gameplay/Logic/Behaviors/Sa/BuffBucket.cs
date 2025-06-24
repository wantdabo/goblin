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
            if (false == stage.SeekBehaviorInfo(actor, out BuffBucketInfo buffbucket)) return;
            if (false == buffbucket.buffdict.TryGetValue(buffid, out var buff)) return;
            
            buffbucket.buffdict.Remove(buffid);
            buffbucket.buffs.Remove(buff);
            stage.RmvActor(buff);
        }

        public void AddBuff(ulong actor, uint buffid, uint layer, ulong duration, List<uint> pipelines = null)
        {
            if (false == stage.SeekBehaviorInfo(actor, out BuffBucketInfo buffbucket)) buffbucket = stage.AddBehaviorInfo<BuffBucketInfo>(actor);
            if (buffbucket.buffdict.ContainsKey(buffid)) return;

            var buff = stage.Spawn(new BuffPrefabInfo
            {
                buffid = buffid,
                layer = layer,
                owner = actor,
                pipelines = pipelines
            });
            buffbucket.buffs.Add(buff);
            buffbucket.buffdict.Add(buffid, buff);
        }

        public BuffInfo GetBuff(ulong actor, uint buffid)
        {
            if (false == stage.SeekBehaviorInfo(actor, out BuffBucketInfo buffbucket)) buffbucket = stage.AddBehaviorInfo<BuffBucketInfo>(actor);
            if (false == buffbucket.buffdict.TryGetValue(buffid, out var buff) || false == stage.SeekBehaviorInfo(buff, out BuffInfo buffinfo)) return null;

            return buffinfo;
        }

        public void AppBuff(ulong actor, uint buffid, uint layer, ulong duration)
        {
            var buffinfo = GetBuff(actor, buffid);
            if (null == buffinfo) return;

            buffinfo.layer += layer;
            buffinfo.duration += duration;
        }

        public void SetBuff(ulong actor, uint buffid, uint layer, ulong duration)
        {
            if (false == stage.SeekBehaviorInfo(actor, out BuffBucketInfo buffbucket)) return;
            if (false == buffbucket.buffdict.TryGetValue(buffid, out var buff) || false == stage.SeekBehaviorInfo(buff, out BuffInfo buffinfo)) return;
            buffinfo.layer = layer;
            buffinfo.duration = duration;
        }
    }
}