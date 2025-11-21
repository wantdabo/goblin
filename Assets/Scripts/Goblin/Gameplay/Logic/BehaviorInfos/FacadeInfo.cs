using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
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
        /// 挂点
        /// </summary>
        public ushort mount { get; set; }
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
        /// 动画更新类型
        /// </summary>
        public byte animticktype { get; set; }
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
            animticktype = ANIM_DEFINE.TICK_AUTOMATIC;
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
            animticktype = ANIM_DEFINE.TICK_AUTOMATIC;
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
            clone.animticktype = animticktype;
            clone.animstate = animstate;
            clone.animname = animname;
            clone.animelapsed = animelapsed;
            clone.effectincrement = effectincrement;
            clone.rmveffects.AddRange(rmveffects);
            clone.effects.AddRange(effects);
            foreach (var kv in effectdict) clone.effectdict.Add(kv.Key, kv.Value);
            
            return clone;
        }
    }
}