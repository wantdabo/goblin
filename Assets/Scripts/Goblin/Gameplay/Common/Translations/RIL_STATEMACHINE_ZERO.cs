using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.Translations.Common;

namespace Goblin.Gameplay.Common.Translations
{
    public struct RIL_STATEMACHINE_ZERO : IRIL
    {
        public ushort id => RILDef.STATEMACHINE_ZERO;
        public uint sid { get; private set; }
        public byte layer { get; private set; }

        public RIL_STATEMACHINE_ZERO(uint sid, byte layer)
        {
            this.sid = sid;
            this.layer = layer;
        }

        public byte[] Serialize()
        {
            throw new System.NotImplementedException();
        }

        public bool Equals(IRIL other)
        {
            if (other is RIL_STATEMACHINE_ZERO _other)
            {
                return sid == _other.sid && layer == _other.layer;
            }

            return false;
        }

        public override string ToString()
        {
            return $"ID -> {id}, State -> {sid}, Layer -> {layer}";
        }
    }
}
