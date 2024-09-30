using Goblin.Gameplay.Logic.Translation.Common;
using System;

namespace Goblin.Gameplay.Logic.Translation
{
    public struct RIL_STATEMACHINE : IRIL
    {
        public ushort id => IRIL.STATEMACHINE;
        public uint sid { get; set; }
        public uint frames { get; set; }
        public byte layer { get; set; }

        public RIL_STATEMACHINE(uint sid, uint frames, byte layer)
        {
            this.sid = sid;
            this.frames = frames;
            this.layer = layer;
        }

        public byte[] Serialize()
        {
            // TODO 序列化为二进制
            return Array.Empty<byte>();
        }

        public bool Equals(IRIL other)
        {
            if (other is RIL_STATEMACHINE ril)
            {
                return sid == ril.sid && layer == ril.layer;
            }
            
            return false;
        }
    }
}
