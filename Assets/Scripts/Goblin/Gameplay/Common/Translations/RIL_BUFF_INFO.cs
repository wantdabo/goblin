using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.Translations.Common;

namespace Goblin.Gameplay.Common.Translations
{
    /// <summary>
    /// Buff 信息渲染指令
    /// </summary>
    public class RIL_BUFF_INFO : IRIL
    {
        public ushort id => RIL_DEFINE.BUFF_INFO;
        
        /// <summary>
        /// BuffID
        /// </summary>
        public uint buffid { get; private set; }
        /// <summary>
        /// Buff 类型
        /// </summary>
        public byte type { get; private set; }
        /// <summary>
        /// Buff 状态
        /// </summary>
        public byte state { get; private set; }
        /// <summary>
        /// Buff 层数
        /// </summary>
        public uint layer { get; private set; }
        /// <summary>
        /// Buff 最大层数
        /// </summary>
        public uint maxlayer { get; private set; }
        /// <summary>
        /// 来源/ActorID
        /// </summary>
        public uint from { get; private set; }
        
        /// <summary>
        /// Buff 信息渲染指令
        /// </summary>
        /// <param name="buffid">BuffID</param>
        /// <param name="type">Buff 类型</param>
        /// <param name="state">Buff 状态</param>
        /// <param name="layer">Buff 层数</param>
        /// <param name="maxlayer">Buff 最大层数</param>
        /// <param name="from">来源/ActorID</param>
        public RIL_BUFF_INFO(uint buffid, byte type, byte state, uint layer, uint maxlayer, uint from)
        {
            this.buffid = buffid;
            this.state = state;
            this.layer = layer;
            this.maxlayer = maxlayer;
            this.from = from;
        }

        public byte[] Serialize()
        {
            throw new System.NotImplementedException();
        }

        public bool Equals(IRIL other)
        {
            if (other is RIL_BUFF_INFO _other)
            {
                return _other.buffid == buffid && _other.layer == layer && _other.state == state;
            }

            return false;
        }
        
        public override string ToString()
        {
            return $"RIL_BUFF_INFO: buffid={buffid}, state={state}, layer={layer}, maxlayer={maxlayer}";
        }
    }
}
