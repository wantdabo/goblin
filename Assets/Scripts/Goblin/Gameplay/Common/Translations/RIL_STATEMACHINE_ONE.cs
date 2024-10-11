using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.Translations.Common;

namespace Goblin.Gameplay.Common.Translations
{
    public struct RIL_STATEMACHINE_ONE : IRIL 
    {
        public ushort id => RILDef.STATEMACHINE_ONE;
        public uint state { get; private set; }
        public uint laststate { get; private set; }
        public byte layer { get; private set; }

        public RIL_STATEMACHINE_ONE(uint state, uint lastsate, byte layer)
        {
            this.state = state;
            this.laststate = lastsate;
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
                return state == _other.state && layer == _other.layer;
            }

            return false;
        }

        public override string ToString()
        {
            return $"ID -> {id}, State -> {state}, LastState -> {laststate}, Layer -> {layer}";
        }
    }
}
