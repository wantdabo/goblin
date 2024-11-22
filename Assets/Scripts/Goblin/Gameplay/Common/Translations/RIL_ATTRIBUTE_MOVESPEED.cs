using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.Translations.Common;
using Kowtow.Math;

namespace Goblin.Gameplay.Common.Translations
{
    /// <summary>
    /// 移动速度渲染指令
    /// </summary>
    public class RIL_ATTRIBUTE_MOVESPEED : IRIL
    {
        public ushort id => RIL_DEFINE.ATTRIBUTE_MOVESPEED;
        
        /// <summary>
        /// 移动速度
        /// </summary>
        public FP movespeed { get; private set; }
        
        public RIL_ATTRIBUTE_MOVESPEED(FP movespeed)
        {
            this.movespeed = movespeed;
        }
        
        public byte[] Serialize()
        {
            throw new System.NotImplementedException();
        }
        
        public bool Equals(IRIL other)
        {
            if (other is RIL_ATTRIBUTE_MOVESPEED _other)
            {
                return _other.movespeed == movespeed;
            }

            return false;
        }

        public override string ToString()
        {
            return $"ID -> {id}, movespeed -> {movespeed}";
        }
    }
}
