using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.BehaviorInfos
{
    /// <summary>
    /// 属性信息
    /// </summary>
    public class AttributeInfo : BehaviorInfo
    {
        /// <summary>
        /// 当前生命值
        /// </summary>
        public uint hp { get; set; }
        /// <summary>
        /// 最大生命值
        /// </summary>
        public uint maxhp { get; set; }
        /// <summary>
        /// 移动速度
        /// </summary>
        public uint movespeed { get; set; }
        /// <summary>
        /// 攻击力
        /// </summary>
        public uint attack { get; set; }
        
        protected override void OnReady()
        {
            OnReset();
        }

        protected override void OnReset()
        {
            hp = 0;
            maxhp = 0;
            movespeed = 0;
            attack = 0;
        }

        protected override BehaviorInfo OnClone()
        {
            var clone = ObjectCache.Get<AttributeInfo>();
            clone.Ready(id);
            clone.hp = hp;
            clone.maxhp = maxhp;
            clone.movespeed = movespeed;
            clone.attack = attack;
            
            return clone;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 31 + id.GetHashCode();
            hash = hash * 31 + hp.GetHashCode();
            hash = hash * 31 + maxhp.GetHashCode();
            hash = hash * 31 + movespeed.GetHashCode();
            hash = hash * 31 + attack.GetHashCode();

            return hash;
        }
    }
}