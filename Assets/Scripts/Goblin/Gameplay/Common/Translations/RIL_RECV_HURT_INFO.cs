using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.Translations.Common;

namespace Goblin.Gameplay.Common.Translations
{
    /// <summary>
    /// 受到伤害渲染指令
    /// </summary>
    public struct RIL_RECV_HURT_INFO : IRIL
    {
        public ushort id => RIL_DEFINE.RECV_HURT_INFO;

        /// <summary>
        /// 暴击
        /// </summary>
        public bool crit { get; private set; }
        /// <summary>
        /// 伤害数值
        /// </summary>
        public uint value { get; private set; }
        /// <summary>
        /// 来源/ActorID
        /// </summary>
        public uint from { get; private set; }
        
        /// <summary>
        /// 受到伤害渲染指令
        /// </summary>
        public RIL_RECV_HURT_INFO(bool crit, uint value, uint from)
        {
            this.crit = crit;
            this.value = value;
            this.from = from;
        }

        public byte[] Serialize()
        {
            throw new System.NotImplementedException();
        }

        public bool Equals(IRIL other)
        {
            if (other is RIL_RECV_HURT_INFO ril)
            {
                return crit == ril.crit && value == ril.value && from == ril.from;
            }
            
            return false;
        }
        
        public override string ToString()
        {
            return $"ID -> {id}, crit -> {crit}, value -> {value}, from -> {from}";
        }
    }
}
