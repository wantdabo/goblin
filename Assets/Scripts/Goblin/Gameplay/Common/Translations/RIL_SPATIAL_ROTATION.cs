using Goblin.Gameplay.Common.Translations.Common;
using TrueSync;
using IRIL = Goblin.Gameplay.Common.Translations.Common.IRIL;

namespace Goblin.Gameplay.Common.Translations
{
    using IRIL = Common.IRIL;

    public struct RIL_SPATIAL_ROTATION : IRIL
    {
        public ushort id => RILDef.SPATIAL_ROTATION;
        public TSQuaternion rotation { get; private set; }

        public RIL_SPATIAL_ROTATION(TSQuaternion rotation)
        {
            this.rotation = rotation;
        }

        public byte[] Serialize()
        {
            throw new System.NotImplementedException();
        }

        public bool Equals(IRIL other)
        {
            if (other is RIL_SPATIAL_ROTATION _other)
            {
                return _other.rotation.Equals(rotation);
            }

            return false;
        }
        
        public override string ToString()
        {
            return $"ID -> {id}, Rotation -> ({rotation.x}, {rotation.y}, {rotation.z}, {rotation.w})";
        }
    }
}
