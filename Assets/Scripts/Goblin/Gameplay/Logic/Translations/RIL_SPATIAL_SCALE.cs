using TrueSync;
using IRIL = Goblin.Gameplay.Logic.Translations.Common.IRIL;

namespace Goblin.Gameplay.Logic.Translations
{
    public struct RIL_SPATIAL_SCALE : IRIL
    {
        public ushort id => IRIL.SPATIAL_SCALE;
        public TSVector scale { get; private set; }        
        
        public RIL_SPATIAL_SCALE(TSVector scale)
        {
            this.scale = scale;
        }
        
        public byte[] Serialize()
        {
            throw new System.NotImplementedException();
        }
        
        public bool Equals(IRIL other)
        {
            if (other is RIL_SPATIAL_SCALE _other)
            {
                return _other.scale.Equals(scale);
            }

            return false;
        }
        
        public override string ToString()
        {
            return $"ID -> {id}, Scale -> ({scale.x}, {scale.y}, {scale.z})";
        }
    }
}
