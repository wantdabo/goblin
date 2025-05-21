using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL.Common;

namespace Goblin.Gameplay.Logic.RIL
{
    /// <summary>
    /// 属性渲染指令
    /// </summary>
    public class RIL_ATTRIBUTE : IRIL
    {
        public override ushort id => RIL_DEFINE.ATTRIBUTE;
        
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
            hp = 0;
            maxhp = 0;
            movespeed = 0;
            attack = 0;
        }

        protected override void OnReset()
        {
            hp = 0;
            maxhp = 0;
            movespeed = 0;
            attack = 0;
        }

        protected override void OnCopy(ref IRIL target)
        {
            if (target is not RIL_ATTRIBUTE ril) return;
            ril.hp = hp;
            ril.maxhp = maxhp;
            ril.movespeed = movespeed;
            ril.attack = attack;
        }

        public override byte[] Serialize()
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return $"RIL_ATTRIBUTE: hp={hp}, maxhp={maxhp}, movespeed={movespeed}, attack={attack}";
        }
    }
}