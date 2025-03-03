﻿using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.Translations.Common;
using Kowtow.Math;

namespace Goblin.Gameplay.Common.Translations
{
    /// <summary>
    /// 空间平移渲染指令
    /// </summary>
    public struct RIL_SPATIAL_POSITION : IRIL
    {
        public ushort id => RIL_DEFINE.SPATIAL_POSITION;
        /// <summary>
        /// 平移
        /// </summary>
        public FPVector3 position { get; private set; }

        /// <summary>
        /// 空间平移渲染指令
        /// </summary>
        /// <param name="position">平移</param>
        public RIL_SPATIAL_POSITION(FPVector3 position)
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
