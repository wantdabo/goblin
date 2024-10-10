using Goblin.Gameplay.Common.Translations.Common;
using IRIL = Goblin.Gameplay.Common.Translations.Common.IRIL;

namespace Goblin.Gameplay.Common.Translations
{
    using IRIL = Common.IRIL;

    public struct RIL_LIVE_BORN : IRIL
    {
        public ushort id => RILDef.LIVE_BORN;
        
        public byte[] Serialize()
        {
            throw new System.NotImplementedException();
        }
        
        public bool Equals(IRIL other)
        {
            return true;
        }
    }
}
