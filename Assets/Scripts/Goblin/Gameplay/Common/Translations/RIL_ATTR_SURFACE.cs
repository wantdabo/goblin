using Goblin.Gameplay.Common.Translations.Common;
using IRIL = Goblin.Gameplay.Common.Translations.Common.IRIL;

namespace Goblin.Gameplay.Common.Translations
{
    using IRIL = Common.IRIL;

    public class RIL_ATTR_SURFACE : IRIL
    {
        public ushort id => RILDef.ATTR_SURFACE;

        public uint model { get; private set; }
        
        public RIL_ATTR_SURFACE(uint model)
        {
            this.model = model;
        }

        public byte[] Serialize()
        {
            throw new System.NotImplementedException();
        }
        
        public bool Equals(IRIL other)
        {
            if (other is RIL_ATTR_SURFACE _other)
            {
                return _other.model == model;
            }

            return false;
        }

        public override string ToString()
        {
            return $"ID -> {id}, model -> {model}";
        }
    }
}
