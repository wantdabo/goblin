using Goblin.Gameplay.Common.Translations;
using Goblin.Gameplay.Common.Translations.Common;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Attributes
{
    /// <summary>
    /// 属性翻译
    /// </summary>
    public class Translator : Translator<Attribute>
    {
        /// <summary>
        /// 生命值
        /// </summary>
        public uint hp { get; set; }
        /// <summary>
        /// 最大生命值
        /// </summary>
        public uint maxhp { get; set; }
        /// <summary>
        /// 移动速度
        /// </summary>
        public FP movespeed { get; set; }
        /// <summary>
        /// 攻击力
        /// </summary>
        public uint attack { get; set; }
        
        protected override void OnRIL()
        {
            if (hp != behavior.hp)
            {
                hp = behavior.hp;
                behavior.actor.stage.rilsync.PushRIL(behavior.actor.id, new RIL_ATTRIBUTE_HP(hp));
            }

            if (maxhp != behavior.maxhp)
            {
                maxhp = behavior.maxhp;
                behavior.actor.stage.rilsync.PushRIL(behavior.actor.id, new RIL_ATTRIBUTE_MAXHP(maxhp));
            }

            if (movespeed != behavior.movespeed)
            {
                movespeed = behavior.movespeed;
                behavior.actor.stage.rilsync.PushRIL(behavior.actor.id, new RIL_ATTRIBUTE_MOVESPEED(movespeed));
            }

            if (attack != behavior.attack)
            {
                attack = behavior.attack;
                behavior.actor.stage.rilsync.PushRIL(behavior.actor.id, new RIL_ATTRIBUTE_ATTACK(attack));
            }
        }
    }
}
