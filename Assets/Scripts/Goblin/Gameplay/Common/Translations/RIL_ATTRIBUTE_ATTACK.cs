﻿using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.Translations.Common;

namespace Goblin.Gameplay.Common.Translations
{
    /// <summary>
    /// 攻击力渲染指令
    /// </summary>
    public struct RIL_ATTRIBUTE_ATTACK : IRIL
    {
        public ushort id => RIL_DEFINE.ATTRIBUTE_ATTACK;

        /// <summary>
        /// 攻击力
        /// </summary>
        public uint attack { get; private set; }
        
        public RIL_ATTRIBUTE_ATTACK(uint attack)
        {
            this.attack = attack;
        }

        public byte[] Serialize()
        {
            throw new System.NotImplementedException();
        }
        
        public bool Equals(IRIL other)
        {
            if (other is RIL_ATTRIBUTE_ATTACK _other)
            {
                return _other.attack == attack;
            }

            return false;
        }

        public override string ToString()
        {
            return $"ID -> {id}, attack -> {attack}";
        }
    }
}
