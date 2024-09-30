using Goblin.Gameplay.Logic.Translation.Common;
using System;
using TrueSync;

namespace Goblin.Gameplay.Logic.Translation
{
    public struct RIL_SPATIAL_SCALE : IRIL
    {
        public ushort id => RIL.SPATIAL_SCALE;
        public TSVector scale { get; private set; }        
        
        public RIL_SPATIAL_SCALE(TSVector scale)
        {
            this.scale = scale;
        }
        
        public byte[] Serialize()
        {
            // TODO 序列化为二进制
            return Array.Empty<byte>();
        }
        
        public bool Equals(IRIL other)
        {
            if (id != other.id) return false;
            var _other = (RIL_SPATIAL_SCALE)other;
            
            return _other.scale.Equals(scale);
        }
    }
}
