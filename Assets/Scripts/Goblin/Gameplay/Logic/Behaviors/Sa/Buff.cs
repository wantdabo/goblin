using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.Prefabs;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Behaviors.Sa
{
    /// <summary>
    /// Buff 行为
    /// </summary>
    public class Buff : Behavior
    {
        /// <summary>
        /// 移除 Buff
        /// </summary>
        /// <param name="owner">Buff 拥有者</param>
        /// <param name="buffid">BuffID</param>
        public void RmvBuff(ulong owner, uint buffid)
        {
            if (false == stage.SeekBehaviorInfo(owner, out BuffBucketInfo buffbucket)) return;
            if (false == buffbucket.buffdict.TryGetValue(buffid, out var buff)) return;
            
            // 移除 Buff
            buffbucket.buffdict.Remove(buffid);
            buffbucket.buffs.Remove(buff);
            stage.RmvActor(buff);
        }

        /// <summary>
        /// 添加 Buff
        /// </summary>
        /// <param name="owner">Buff 拥有者</param>
        /// <param name="buffid">BuffID</param>
        /// <param name="layer">Buff 层数</param>
        /// <param name="lifetime">Buff 生命周期</param>
        public void AddBuff(ulong owner, uint buffid, uint layer, FP lifetime)
        {
            if (false == stage.SeekBehaviorInfo(owner, out BuffBucketInfo buffbucket)) buffbucket = stage.AddBehaviorInfo<BuffBucketInfo>(owner);
            
            // 检查是否已经存在 Buff
            var buffinfo = GetBuff(owner, buffid);
            if (null != buffinfo)
            {
                SetBuff(buffinfo.owner, buffinfo.buffid, buffinfo.layer += layer, buffinfo.lifetime);
                return;
            }

            // 创建新的 Buff
            var buff = stage.Spawn(new BuffPrefabInfo
            {
                buffid = buffid,
                layer = layer,
                lifetime = lifetime,
                owner = owner,
            });
            buffbucket.buffs.Add(buff);
            buffbucket.buffdict.Add(buffid, buff);
        }

        public void SetBuff(ulong owner, uint buffid, uint layer, FP lifetime)
        {
            if (false == stage.SeekBehaviorInfo(owner, out BuffBucketInfo buffbucket)) return;
            if (false == buffbucket.buffdict.TryGetValue(buffid, out var buff) || false == stage.SeekBehaviorInfo(buff, out BuffInfo buffinfo)) return;
            buffinfo.layer = layer;
            buffinfo.lifetime = lifetime;
        }

        /// <summary>
        /// 获取 Buff
        /// </summary>
        /// <param name="owner">Buff 拥有者</param>
        /// <param name="buffid">BuffID</param>
        /// <returns>Buff 信息</returns>
        public BuffInfo GetBuff(ulong owner, uint buffid)
        {
            if (false == stage.SeekBehaviorInfo(owner, out BuffBucketInfo buffbucket)) return default;
            if (false == buffbucket.buffdict.TryGetValue(buffid, out var buff) || false == stage.SeekBehaviorInfo(buff, out BuffInfo buffinfo)) return default;

            return buffinfo;
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