﻿using IRIL = Goblin.Gameplay.Logic.Translations.Common.IRIL;

namespace Goblin.Gameplay.Logic.Translations
{
    public struct RIL_LIVE_DEAD : IRIL
    {
        public ushort id => IRIL.LIVE_DEAD;

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