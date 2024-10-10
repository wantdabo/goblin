using Goblin.Gameplay.Common.Translations.Common;
using IRIL = Goblin.Gameplay.Common.Translations.Common.IRIL;

namespace Goblin.Gameplay.Common.Translations
{
    using IRIL = Common.IRIL;

    public struct RIL_LIVE_DEAD : IRIL
    {
        public ushort id => RILDef.LIVE_DEAD;

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
