using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.Translations.Common;

namespace Goblin.Gameplay.Common.Translations
{
    /// <summary>
    /// 受到治疗渲染指令
    /// </summary>
    public struct RIL_RECV_CURE_INFO : IRIL
    {
        public ushort id => RIL_DEFINE.RECV_CURE_INFO;

        /// <summary>
        /// 治疗数值
        /// </summary>
        public uint cure;
        /// <summary>
        /// 来源/ActorID
        /// </summary>
        public uint from;

        /// <summary>
        /// 受到治疗渲染指令
        /// </summary>
        public RIL_RECV_CURE_INFO(uint cure, uint from)
        {
            this.cure = cure;
            this.from = from;
        }

        public byte[] Serialize()
        {
            throw new System.NotImplementedException();
        }

        public bool Equals(IRIL other)
        {
            if (other is RIL_RECV_CURE_INFO ril)
            {
                return cure == ril.cure && from == ril.from;
            }
            
            return false;
        }

        public override string ToString()
        {
            return $"ID -> {id}, cure -> {cure}, from -> {from}";
        }
    }
}
