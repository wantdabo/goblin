using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.BehaviorInfos
{
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
        
        protected override void OnReady()
        {
            model = 0;
            animstate = 0;
            animname = null;
            animelapsed = FP.Zero;
        }

        protected override void OnReset()
        {
            model = 0;
            animstate = 0;
            animname = null;
            animelapsed = FP.Zero;
        }

        protected override BehaviorInfo OnClone()
        {
            var clone = ObjectCache.Ensure<FacadeInfo>();
            clone.Ready(actor);
            clone.model = model;
            clone.animstate = animstate;
            clone.animname = animname;
            clone.animelapsed = animelapsed;
            
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
            
            return hash;
        }
    }
}