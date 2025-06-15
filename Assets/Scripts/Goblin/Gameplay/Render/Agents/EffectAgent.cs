using System.Collections.Generic;
using Goblin.Common;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Render.Common;
using Goblin.Gameplay.Render.Common.Extensions;
using Goblin.Gameplay.Render.Core;
using UnityEngine;

namespace Goblin.Gameplay.Render.Agents
{
    /// <summary>
    /// 特效代理
    /// </summary>
    public class EffectAgent : Agent
    {
        /// <summary>
        /// Effect 根
        /// </summary>
        private static GameObject root = new("Effect");
        static EffectAgent()
        {
            root.transform.SetParent(GameObject.Find("Gameplay").transform, false);
            root.transform.localPosition = Vector3.zero;
            root.transform.localScale = Vector3.one;
        }
        
        private Dictionary<uint, (EffectInfo info, EffectController controller)> effects { get; set; }
        
        protected override void OnReady()
        {
            effects = ObjectPool.Ensure<Dictionary<uint, (EffectInfo info, EffectController)>>();
            WatchRIL<RIL_FACADE_EFFECT>(OnRILFacadeEffect);
        }

        protected override void OnReset()
        {
            var rmveffects = ObjectPool.Ensure<List<uint>>();
            foreach (var kv in effects) rmveffects.Add(kv.Key);
            foreach (var id in rmveffects) RecycleEffect(id);
            rmveffects.Clear();
            ObjectPool.Set(rmveffects);
            
            effects.Clear();
            ObjectPool.Set(effects);
        }

        /// <summary>
        /// 创建特效
        /// </summary>
        /// <param name="info">EffectInfo</param>
        private void CreateEffect(EffectInfo info)
        {
            if (effects.ContainsKey(info.id)) return;
            if (false == world.engine.cfg.location.EffectInfos.TryGetValue(info.effect, out var effcfg)) return;
            
            var controller = ObjectPool.Get<EffectController>(effcfg.Res);
            if (null == controller)
            {
                var effgo = world.engine.gameres.location.LoadEffectSync(effcfg.Res);
                controller = effgo.GetComponent<EffectController>();
                controller.transform.SetParent(root.transform, false);
            }
            controller.gameObject.SetActive(true);
            controller.Simulate(info.elapsed.AsFloat());
            effects.Add(info.id, (info, controller));
        }

        /// <summary>
        /// 回收特效
        /// </summary>
        /// <param name="id">EffectID</param>
        private void RecycleEffect(uint id)
        {
            if (false == effects.TryGetValue(id, out var effect)) return;
            effects.Remove(id);

            if (false == world.engine.cfg.location.EffectInfos.TryGetValue(effect.info.effect, out var effcfg)) return;
            effect.controller.gameObject.SetActive(false);
            effect.controller.Reset();
            ObjectPool.Set(effect.controller, effcfg.Res);
        }

        private void OnRILFacadeEffect(RIL_FACADE_EFFECT ril)
        {
            // 回收特效
            var rmveffects = ObjectPool.Ensure<List<uint>>();
            foreach (var kv in effects) if (false == ril.effectdict.ContainsKey(kv.Key)) rmveffects.Add(kv.Key);
            foreach (var id in rmveffects) RecycleEffect(id);
            rmveffects.Clear();
            ObjectPool.Set(rmveffects);
            
            // 添加/更新特效
            foreach (var kv in ril.effectdict)
            {
                // 如果特效已存在，则更新
                if (effects.TryGetValue(kv.Key, out var effect))
                {
                    effects.Remove(kv.Key);
                    effects.Add(kv.Key, (kv.Value, effect.controller));
                    continue;
                }
                
                // 如果特效不存在，则创建
                CreateEffect(kv.Value);
            }
        }
        
        protected override void OnChase(float tick, float timescale)
        {
            base.OnChase(tick, timescale);
            foreach (var kv in effects)
            {
                var info = kv.Value.info;
                var controller = kv.Value.controller;
                
                var followpos = info.position.ToVector3();
                var followeuler = info.euler.ToVector3();
                var followscale = info.scale.AsFloat();
                switch (info.follow)
                {
                    case EFFECT_DEFINE.FOLLOW_ACTOR:
                        if (world.rilbucket.SeekRIL(actor, out RIL_SPATIAL spatial))
                        {
                            var position = spatial.position.ToVector3();
                            var euler = spatial.euler.ToVector3();
                            var scale = spatial.scale.AsFloat();

                            followpos = position + Quaternion.Euler(euler) * followpos;
                            followeuler += euler;
                            followscale *= scale;
                        }
                        break;
                    case EFFECT_DEFINE.FOLLOW_MOUNT:
                        // TODO 获取挂载点位置
                        break;
                }

                if (EFFECT_DEFINE.FOLLOW_NONE != info.followmask)
                {
                    if (EFFECT_DEFINE.FOLLOW_POSITION == (info.followmask & EFFECT_DEFINE.FOLLOW_POSITION))
                    {
                        controller.transform.position = followpos;
                    }
                    if (EFFECT_DEFINE.FOLLOW_ROTATION == (info.followmask & EFFECT_DEFINE.FOLLOW_ROTATION))
                    {
                        controller.transform.eulerAngles = followeuler;
                    }
                    if (EFFECT_DEFINE.FOLLOW_SCALE == (info.followmask & EFFECT_DEFINE.FOLLOW_SCALE))
                    {
                        controller.transform.localScale = Vector3.one * followscale;
                    }
                }

                controller.Simulate(Mathf.Clamp(controller.time + tick * timescale, 0, info.elapsed.AsFloat()));
            }
        }
    }
}