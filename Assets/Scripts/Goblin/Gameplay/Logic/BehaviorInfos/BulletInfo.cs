using System.Collections.Generic;
using Goblin.Gameplay.Logic.Behaviors;
using Goblin.Gameplay.Logic.Behaviors.Sa;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.BehaviorInfos
{
    /// <summary>
    /// 子弹信息
    /// </summary>
    public class BulletInfo : BehaviorInfo
    {
        /// <summary>
        /// 子弹拥有者
        /// </summary>
        public ulong owner { get; set; }
        /// <summary>
        /// 正在进行的子弹管线 ActorID
        /// </summary>
        public ulong flow { get; set; }
        /// <summary>
        /// 子弹伤害强度
        /// </summary>
        public FP strength { get; set; }
        /// <summary>
        /// 子弹的速度
        /// </summary>
        public FP speed { get; set; }
        /// <summary>
        /// 子弹的伤害
        /// </summary>
        public DamageInfo damage { get; set; }
        
        protected override void OnReady()
        {
            OnReset();
        }

        protected override void OnReset()
        {
            owner = 0;
            flow = 0;
            strength = 0;
            speed = 0;
            damage = default;
        }

        protected override BehaviorInfo OnClone()
        {
            var clone = ObjectCache.Ensure<BulletInfo>();
            clone.Ready(actor);
            clone.owner = owner;
            clone.flow = flow;
            clone.strength = strength;
            clone.speed = speed;
            clone.damage = damage;
            
            return clone;
        }
    }
}