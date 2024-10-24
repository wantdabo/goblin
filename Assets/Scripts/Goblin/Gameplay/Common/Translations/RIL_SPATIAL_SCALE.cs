using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.Translations.Common;
using TrueSync;

namespace Goblin.Gameplay.Common.Translations
{
    /// <summary>
    /// 空间缩放渲染指令
    /// </summary>
    public struct RIL_SPATIAL_SCALE : IRIL
    {
        public ushort id => RILDef.SPATIAL_SCALE;
        /// <summary>
        /// 缩放
        /// </summary>
        public TSVector scale { get; private set; }        
        
        /// <summary>
        /// 空间缩放渲染指令
        /// </summary>
        /// <param name="scale"></param>
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
