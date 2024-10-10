using Goblin.Gameplay.Common.Translations.Common;
using TrueSync;
using IRIL = Goblin.Gameplay.Common.Translations.Common.IRIL;

namespace Goblin.Gameplay.Common.Translations
{
    using IRIL = Common.IRIL;

    public struct RIL_SPATIAL_POSITION : IRIL
    {
        public ushort id => RILDef.SPATIAL_POSITION;
        public TSVector position { get; private set; }

        public RIL_SPATIAL_POSITION(TSVector position)
        {
            this.position = position;
        }

        public byte[] Serialize()
        {
            throw new System.NotImplementedException();
        }

        public bool Equals(IRIL other)
        {
            if (other is RIL_SPATIAL_POSITION _other)
            {
                return _other.position.Equals(position);
            }

            return false;
        }

        public override string ToString()
        {
            return $"ID -> {id}, Position -> ({position.x}, {position.y}, {position.z})";
        }
    }
}
