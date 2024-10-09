using TrueSync;
using IRIL = Goblin.Gameplay.Logic.Translations.Common.IRIL;

namespace Goblin.Gameplay.Logic.Translations
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
