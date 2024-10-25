using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.Translations.Common;

namespace Goblin.Gameplay.Common.Translations
{
    /// <summary>
    /// 最大生命中渲染指令
    /// </summary>
    public class RIL_ATTRIBUTE_MAXHP : IRIL
    {
        public ushort id => RILDef.ATTRIBUTE_MAXHP;

        /// <summary>
        /// 最大生命值
        /// </summary>
        public uint maxhp { get; set; }
        
        public RIL_ATTRIBUTE_MAXHP(uint maxhp)
        {
            this.maxhp = maxhp;
        }

        public byte[] Serialize()
        {
            throw new System.NotImplementedException();
        }
        
        public bool Equals(IRIL other)
        {
            if (other is RIL_ATTRIBUTE_MAXHP _other)
            {
                return _other.maxhp == maxhp;
            }

            return false;
        }

        public override string ToString()
        {
            return $"ID -> {id}, maxhp -> {maxhp}";
        }
    }
}
