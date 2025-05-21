using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL.Common;

namespace Goblin.Gameplay.Logic.RIL
{
    /// <summary>
    /// 驱动渲染指令
    /// </summary>
    public class RIL_TICKER : IRIL
    {
        public override ushort id => RIL_DEFINE.TICKER;
        /// <summary>
        /// 时间缩放
        /// </summary>
        public uint timescale { get; set; }

        protected override void OnReady()
        {
            timescale = 1;
        }

        protected override void OnReset()
        {
            timescale = 1;
        }

        protected override void OnCopy(ref IRIL target)
        {
            if (target is not RIL_TICKER ril) return;
            ril.timescale = timescale;
        }

        public override byte[] Serialize()
        {
            throw new System.NotImplementedException();
        }
        
        public override string ToString()
        {
            return $"RIL_TICKER: timescale={timescale}";
        }
    }
}