using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.Translations.Common;

namespace Goblin.Gameplay.Common.Translations
{
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
