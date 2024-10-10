using Goblin.Gameplay.Common.Translations.Common;
using IRIL = Goblin.Gameplay.Common.Translations.Common.IRIL;

namespace Goblin.Gameplay.Common.Translations
{
    using IRIL = Common.IRIL;

    public struct RIL_STATEMACHINE_ONE : IRIL 
    {
        public ushort id => RILDef.STATEMACHINE_ONE;
        public uint sid { get; private set; }
        public uint frames { get; private set; }
        public byte layer { get; private set; }

        public RIL_STATEMACHINE_ONE(uint sid, uint frames, byte layer)
        {
            this.sid = sid;
            this.frames = frames;
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
                return sid == _other.sid && frames == _other.frames && layer == _other.layer;
            }
            
            return false;
        }
        
        public override string ToString()
        {
            return $"ID -> {id}, State -> {sid}, Frames -> {frames}, Layer -> {layer}";
        }
    }
}
