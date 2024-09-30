using Goblin.Gameplay.Logic.Translation.Common;
using System;
using TrueSync;

namespace Goblin.Gameplay.Logic.Translation
{
    public struct RIL_SPATIAL_POSITION : IRIL
    {
        public ushort id => IRIL.SPATIAL_POSITION;
        public TSVector position { get; private set; }

        public RIL_SPATIAL_POSITION(TSVector position)
        {
            this.position = position;
        }

        public byte[] Serialize()
        {
            // TODO 序列化为二进制
            return Array.Empty<byte>();
        }

        public bool Equals(IRIL other)
        {
            if (other is RIL_SPATIAL_POSITION _other)
            {
                return _other.position.Equals(position);
            }

            return false;
        }
    }
}
