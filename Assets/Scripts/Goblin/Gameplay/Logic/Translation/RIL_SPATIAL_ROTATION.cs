using Goblin.Gameplay.Logic.Translation.Common;
using System;
using TrueSync;

namespace Goblin.Gameplay.Logic.Translation
{
    public struct RIL_SPATIAL_ROTATION : IRIL
    {
        public ushort id => IRIL.SPATIAL_ROTATION;
        public TSQuaternion rotation { get; private set; }

        public RIL_SPATIAL_ROTATION(TSQuaternion rotation)
        {
            this.rotation = rotation;
        }

        public byte[] Serialize()
        {
            // TODO 序列化为二进制
            return Array.Empty<byte>();
        }

        public bool Equals(IRIL other)
        {
            if (other is RIL_SPATIAL_ROTATION _other)
            {
                return _other.rotation.Equals(rotation);
            }

            return false;
        }
    }
}
