using Goblin.Gameplay.Logic.Translation.Common;
using System;

namespace Goblin.Gameplay.Logic.Translation
{
    public struct RIL_STATEMACHINE_ZERO : IRIL
    {
        public ushort id => IRIL.STATEMACHINE_ZERO;
        public uint sid { get; private set; }
        public uint frames { get; private set; }
        public byte layer { get; private set; }

        public RIL_STATEMACHINE_ZERO(uint sid, uint frames, byte layer)
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
