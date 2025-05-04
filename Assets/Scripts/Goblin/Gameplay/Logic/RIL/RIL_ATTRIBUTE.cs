using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL.Common;

namespace Goblin.Gameplay.Logic.RIL
{
    /// <summary>
    /// 属性渲染指令
    /// </summary>
    public struct RIL_ATTRIBUTE : IRIL
    {
        public ushort id => RIL_DEFINE.ATTRIBUTE;
        
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
        
        public byte[] Serialize()
        {
            throw new System.NotImplementedException();
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

        public override string ToString()
        {
            return $"RIL_ATTRIBUTE: hp={hp}, maxhp={maxhp}, movespeed={movespeed}, attack={attack}";
        }
    }
}