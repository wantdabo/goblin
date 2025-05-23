using System.Collections.Generic;
using Goblin.Gameplay.Logic.Behaviors;
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
            speed = 0;
            damage = default;
        }

        protected override BehaviorInfo OnClone()
        {
            var clone = ObjectCache.Ensure<BulletInfo>();
            clone.Ready(id);
            clone.owner = owner;
            clone.flow = flow;
            clone.speed = speed;
            clone.damage = damage;
            
            return clone;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 31 + owner.GetHashCode();
            hash = hash * 31 + flow.GetHashCode();
            hash = hash * 31 + strength.GetHashCode();
            hash = hash * 31 + speed.GetHashCode();
            hash = hash * 31 + damage.GetHashCode();

            return hash;
        }
    }
}