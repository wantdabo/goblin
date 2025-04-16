using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL.Common;

namespace Goblin.Gameplay.Logic.RIL
{
    /// <summary>
    /// 座位渲染指令
    /// </summary>
    public struct RIL_SEAT : IRIL
    {
        public ushort id => RIL_DEFINE.SEAT;
        
        /// <summary>
        /// 座位 ID
        /// </summary>
        public ulong seat { get; set; }
        
        /// <summary>
        /// 座位渲染指令
        /// </summary>
        /// <param name="seat">座位 ID</param>
        public RIL_SEAT(ulong seat)
        {
            this.seat = seat;
        }
        
        public byte[] Serialize()
        {
            throw new System.NotImplementedException();
        }

        public bool Equals(IRIL other)
        {
            var ril = (RIL_SEAT)other;
            
            return seat.Equals(ril.seat);
        }

        public override string ToString()
        {
            return $"RIL_SEAT: seat={seat}";
        }
    }
}