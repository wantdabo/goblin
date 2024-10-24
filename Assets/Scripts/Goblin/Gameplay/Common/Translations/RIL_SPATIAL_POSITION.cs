using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.Translations.Common;
using TrueSync;

namespace Goblin.Gameplay.Common.Translations
{
    /// <summary>
    /// 空间平移渲染指令
    /// </summary>
    public struct RIL_SPATIAL_POSITION : IRIL
    {
        public ushort id => RILDef.SPATIAL_POSITION;
        /// <summary>
        /// 平移
        /// </summary>
        public TSVector position { get; private set; }

        /// <summary>
        /// 空间平移渲染指令
        /// </summary>
        /// <param name="position">平移</param>
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
