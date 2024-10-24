using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.Translations.Common;
using TrueSync;

namespace Goblin.Gameplay.Common.Translations
{
    /// <summary>
    /// 空间旋转渲染指令
    /// </summary>
    public struct RIL_SPATIAL_ROTATION : IRIL
    {
        public ushort id => RILDef.SPATIAL_ROTATION;
        /// <summary>
        /// 旋转
        /// </summary>
        public TSQuaternion rotation { get; private set; }

        /// <summary>
        /// 空间旋转渲染指令
        /// </summary>
        /// <param name="rotation">旋转</param>
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
