using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL.Common;

namespace Goblin.Gameplay.Logic.RIL
{
    /// <summary>
    /// 驱动渲染指令
    /// </summary>
    public struct RIL_TICKER : IRIL
    {
        public ushort id => RIL_DEFINE.TICKER;
        /// <summary>
        /// 时间缩放
        /// </summary>
        public uint timescale { get; set; }

        public byte[] Serialize()
        {
            throw new System.NotImplementedException();
        }

        public override int GetHashCode()
        {
            return id.GetHashCode() ^ timescale.GetHashCode();
        }

        public override string ToString()
        {
            return $"RIL_TICKER: timescale={timescale}";
        }
    }
}