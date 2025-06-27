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
        public void RmvBuff(ulong owner, int buffid)
        {
            var buffinfo = GetBuff(owner, buffid);
            if (null == buffinfo) return;
            if (false == stage.SeekBehaviorInfo(owner, out BuffBucketInfo buffbucket)) return;
            
            if (buffinfo.enchanted) EraseEnchant(buffinfo);
            // 移除 Buff
            buffbucket.buffdict.Remove(buffid);
            buffbucket.buffs.Remove(buffinfo.actor);
            stage.RmvActor(buffinfo.actor);
        }

        /// <summary>
        /// 添加 Buff
        /// </summary>
        /// <param name="owner">Buff 拥有者</param>
        /// <param name="buffid">BuffID</param>
        /// <param name="layer">Buff 层数</param>
        /// <param name="lifetime">Buff 生命周期</param>
        public void AddBuff(ulong owner, int buffid, int layer, FP lifetime)
        {
            if (false == stage.cfg.location.BuffInfos.TryGetValue(buffid, out var buffcfg)) return;
            if (false == stage.SeekBehaviorInfo(owner, out BuffBucketInfo buffbucket)) buffbucket = stage.AddBehaviorInfo<BuffBucketInfo>(owner);
            
            // 检查是否已经存在 Buff
            var buffinfo = GetBuff(owner, buffid);
            if (null == buffinfo)
            {
                // 读取配置表中 pipelines
                List<uint> pipelines = default;
                if (0 != buffcfg.Pipelines.Count)
                {
                    pipelines = ObjectCache.Ensure<List<uint>>();
                    foreach (var pipeline in buffcfg.Pipelines) pipelines.Add((uint)pipeline);
                }
                // 创建新的 Buff
                var buff = stage.Spawn(new BuffPrefabInfo
                {
                    buffid = buffid,
                    owner = owner,
                    pipelines = pipelines,
                });
                if (null != pipelines)
                {
                    pipelines.Clear();
                    ObjectCache.Set(pipelines);
                }
                
                buffbucket.buffs.Add(buff);
                buffbucket.buffdict.Add(buffid, buff);
                buffinfo = GetBuff(owner, buffid);
            }
            
            SetBuff(buffinfo, buffinfo.layer + layer, buffinfo.lifetime + lifetime);
        }

        /// <summary>
        /// 设置 Buff
        /// </summary>
        /// <param name="buffinfo">Buff 信息</param>
        /// <param name="layer">Buff 层数</param>
        /// <param name="lifetime">Buff 生命周期</param>
        public void SetBuff(BuffInfo buffinfo, int layer, FP lifetime)
        {
            if (buffinfo.layer != layer)
            {
                buffinfo.layer = layer;
                if (buffinfo.enchanted) EraseEnchant(buffinfo);
                StampEnchant(buffinfo);
            }
            buffinfo.lifetime = lifetime;
        }

        /// <summary>
        /// 获取 Buff
        /// </summary>
        /// <param name="owner">Buff 拥有者</param>
        /// <param name="buffid">BuffID</param>
        /// <returns>Buff 信息</returns>
        public BuffInfo GetBuff(ulong owner, int buffid)
        {
            if (false == stage.cfg.location.BuffInfos.TryGetValue(buffid, out _)) return default;

            if (false == stage.SeekBehaviorInfo(owner, out BuffBucketInfo buffbucket)) return default;
            if (false == buffbucket.buffdict.TryGetValue(buffid, out var buff) || false == stage.SeekBehaviorInfo(buff, out BuffInfo buffinfo)) return default;

            return buffinfo;
        }

        /// <summary>
        /// 擦除 Buff 属性增幅
        /// </summary>
        /// <param name="buffinfo">Buff 信息</param>
        private void EraseEnchant(BuffInfo buffinfo)
        {
            if (false == stage.SeekBehaviorInfo(buffinfo.owner, out AttributeInfo attribute)) return;
            if (false == buffinfo.enchanted) return;
            buffinfo.enchanted = false;
            EnchantToAttribute(attribute, buffinfo, false);
        }

        /// <summary>
        /// 印下 Buff 属性增幅
        /// </summary>
        /// <param name="buffinfo">Buff 信息</param>
        private void StampEnchant(BuffInfo buffinfo)
        {
            if (false == stage.SeekBehaviorInfo(buffinfo.owner, out AttributeInfo attribute)) return;
            if (buffinfo.enchanted) return;
            buffinfo.enchanted = true;
            EnchantToAttribute(attribute, buffinfo, true);
        }

        /// <summary>
        /// 将 Buff 属性增幅应用到属性上
        /// </summary>
        /// <param name="attribute">属性信息</param>
        /// <param name="buffinfo">Buff 信息</param>
        /// <param name="flag">标记 (TRUE 叠加/FALSE 移除)</param>
        private void EnchantToAttribute(AttributeInfo attribute, BuffInfo buffinfo, bool flag)
        {
            if (false == stage.cfg.location.BuffInfos.TryGetValue(buffinfo.buffid, out var buffcfg)) return;

            for (int i = 0; i < buffcfg.EnchantMains.Count; i += 2)
            {
                var key = (ushort) buffcfg.EnchantMains[i];
                var value = buffcfg.EnchantMains[i + 1] * buffinfo.layer;
                value = flag ? value : -value;
                stage.attrc.ChangeAttributeValue(attribute, key, value);
            }
            
            for (int i = 0; i < buffcfg.EnchantScales.Count; i += 2)
            {
                var key = (ushort) buffcfg.EnchantScales[i];
                var value = buffcfg.EnchantScales[i + 1] * buffinfo.layer;
                value = flag ? value : -value;
                stage.attrc.ChangeAttributeScaleValue(attribute, key, value);
            }
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
                    SetBuff(buffinfo, buffinfo.layer, buffinfo.lifetime - bufftick);
                    if (FP.Zero >= buffinfo.lifetime) RmvBuff(buffbucket.actor, buffinfo.buffid);
                }
                buffs.Clear();
                ObjectCache.Set(buffs);
            }
        }
    }
}