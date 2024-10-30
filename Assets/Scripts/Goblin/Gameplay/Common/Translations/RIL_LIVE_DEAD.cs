using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.Translations.Common;

namespace Goblin.Gameplay.Common.Translations
{
    /// <summary>
    /// Actor 死亡渲染指令
    /// </summary>
    public struct RIL_LIVE_DEAD : IRIL
    {
        public ushort id => RIL_DEFINE.LIVE_DEAD;

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
