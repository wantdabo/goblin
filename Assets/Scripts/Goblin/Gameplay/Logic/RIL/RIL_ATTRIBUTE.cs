using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL.Common;

namespace Goblin.Gameplay.Logic.RIL
{
    /// <summary>
    /// 属性渲染指令
    /// </summary>
    public class RIL_ATTRIBUTE : IRIL
    {
        public ushort id => RIL_DEFINE.ATTRIBUTE;
        
        /// <summary>
        /// 当前生命值
        /// </summary>
        public uint hp { get; private set; }
        /// <summary>
        /// 最大生命值
        /// </summary>
        public uint maxhp { get; private set; }
        /// <summary>
        /// 移动速度
        /// </summary>
        public uint movespeed { get; private set; }
        /// <summary>
        /// 攻击力
        /// </summary>
        public uint attack { get; private set; }
        
        /// <summary>
        /// 属性渲染指令
        /// </summary>
        /// <param name="hp">当前生命值</param>
        /// <param name="maxhp">最大生命值</param>
        /// <param name="movespeed">移动速度</param>
        /// <param name="attack">攻击力</param>
        public RIL_ATTRIBUTE(uint hp, uint maxhp, uint movespeed, uint attack)
        {
            this.hp = hp;
            this.maxhp = maxhp;
            this.movespeed = movespeed;
            this.attack = attack;
        }
        
        public byte[] Serialize()
        {
            throw new System.NotImplementedException();
        }

        public bool Equals(IRIL other)
        {
            RIL_ATTRIBUTE ril = (RIL_ATTRIBUTE)other;
            
            return hp == ril.hp && maxhp == ril.maxhp && movespeed == ril.movespeed && attack == ril.attack;
        }
        
        public override string ToString()
        {
            return $"RIL_ATTRIBUTE: hp={hp}, maxhp={maxhp}, movespeed={movespeed}, attack={attack}";
        }
    }
}