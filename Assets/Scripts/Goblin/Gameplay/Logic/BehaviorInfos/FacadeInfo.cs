using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.BehaviorInfos
{
    /// <summary>
    /// 特效信息
    /// </summary>
    public struct EffectInfo
    {
        /// <summary>
        /// 特效 ID
        /// </summary>
        public uint id { get; set; }
        /// <summary>
        /// 流逝时间
        /// </summary>
        public FP elapsed { get; set; }
        
        /// <summary>
        /// 特效资源 ID
        /// </summary>
        public int effect { get; set; }
        /// <summary>
        /// 特效类型
        /// </summary>
        public byte type { get; set; }
        /// <summary>
        /// 特效跟随
        /// </summary>
        public byte follow { get; set; }
        /// <summary>
        /// 特效跟随掩码
        /// </summary>
        public int followmask { get; set; }
        /// <summary>
        /// 特效持续时间
        /// </summary>
        public FP duration { get; set; }
        /// <summary>
        /// 特效位置
        /// </summary>
        public FPVector3 position { get; set; }
        /// <summary>
        /// 特效旋转
        /// </summary>
        public FPVector3 euler { get; set; }
        /// <summary>
        /// 特效缩放
        /// </summary>
        public FP scale { get; set; }
    }

    /// <summary>
    /// 外观信息
    /// </summary>
    public class FacadeInfo : BehaviorInfo
    {
        /// <summary>
        /// 模型 ID
        /// </summary>
        public int model { get; set; }
        /// <summary>
        /// 动画状态
        /// </summary>
        public byte animstate { get; set; }
        /// <summary>
        /// 动画名称
        /// </summary>
        public string animname { get; set; }
        /// <summary>
        /// 流逝时间
        /// </summary>
        public FP animelapsed { get; set; }
        /// <summary>
        /// 特效增量 ID
        /// </summary>
        public uint effectincrement { get; set; }
        /// <summary>
        /// 移除特效列表
        /// </summary>
        public List<uint> rmveffects { get; set; }
        /// <summary>
        /// 特效列表
        /// </summary>
        public List<uint> effects { get; set; }
        /// <summary>
        /// 特效字典
        /// </summary>
        public Dictionary<uint, EffectInfo> effectdict { get; set; }
        
        protected override void OnReady()
        {
            model = 0;
            animstate = 0;
            animname = null;
            animelapsed = FP.Zero;
            effectincrement = 0;
            rmveffects = ObjectCache.Ensure<List<uint>>();
            effects = ObjectCache.Ensure<List<uint>>();
            effectdict = ObjectCache.Ensure<Dictionary<uint, EffectInfo>>();
        }

        protected override void OnReset()
        {
            model = 0;
            animstate = 0;
            animname = null;
            animelapsed = FP.Zero;
            effectincrement = 0;
            rmveffects.Clear();
            ObjectCache.Set(rmveffects);
            effects.Clear();
            ObjectCache.Set(effects);
            effectdict.Clear();
            ObjectCache.Set(effectdict);
        }

        protected override BehaviorInfo OnClone()
        {
            var clone = ObjectCache.Ensure<FacadeInfo>();
            clone.Ready(actor);
            clone.model = model;
            clone.animstate = animstate;
            clone.animname = animname;
            clone.animelapsed = animelapsed;
            clone.effectincrement = effectincrement;
            clone.rmveffects = ObjectCache.Ensure<List<uint>>();
            clone.rmveffects.AddRange(rmveffects);
            clone.effects = ObjectCache.Ensure<List<uint>>();
            clone.effects.AddRange(effects);
            clone.effectdict = ObjectCache.Ensure<Dictionary<uint, EffectInfo>>();
            foreach (var kv in effectdict) clone.effectdict.Add(kv.Key, kv.Value);
            
            return clone;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 31 + actor.GetHashCode();
            hash = hash * 31 + model.GetHashCode();
            hash = hash * 31 + animstate.GetHashCode();
            hash = hash * 31 + (animname != null ? animname.GetHashCode() : 0);
            hash = hash * 31 + animelapsed.GetHashCode();
            hash = hash * 31 + effectincrement.GetHashCode();
            foreach (var rmveffect in rmveffects) hash = hash * 31 + rmveffect.GetHashCode();
            foreach (var id in effects)
            {
                hash = hash * 31 + id.GetHashCode();
                if (effectdict.TryGetValue(id, out var eff))
                {
                    hash = hash * 31 + eff.id.GetHashCode();
                    hash = hash * 31 + eff.effect.GetHashCode();
                    hash = hash * 31 + eff.type.GetHashCode();
                    hash = hash * 31 + eff.follow.GetHashCode();
                    hash = hash * 31 + eff.followmask.GetHashCode();
                    hash = hash * 31 + eff.duration.GetHashCode();
                    hash = hash * 31 + eff.position.GetHashCode();
                    hash = hash * 31 + eff.euler.GetHashCode();
                    hash = hash * 31 + eff.scale.GetHashCode();
                }
            }
            
            return hash;
        }
    }
}