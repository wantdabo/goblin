using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.Translations.Common;

namespace Goblin.Gameplay.Common.Translations
{
    /// <summary>
    /// 生命值渲染指令
    /// </summary>
    public class RIL_ATTRIBUTE_HP : IRIL
    {
        public ushort id => RIL_DEFINE.ATTRIBUTE_HP;
        /// <summary>
        /// 生命值
        /// </summary>
        public uint hp { get; set; }
        
        public RIL_ATTRIBUTE_HP(uint hp)
        {
            this.hp = hp;
        }

        public byte[] Serialize()
        {
            throw new System.NotImplementedException();
        }
        
        public bool Equals(IRIL other)
        {
            if (other is RIL_ATTRIBUTE_HP _other)
            {
                return _other.hp == hp;
            }

            return false;
        }

        public override string ToString()
        {
            return $"ID -> {id}, hp -> {hp}";
        }
    }
}
