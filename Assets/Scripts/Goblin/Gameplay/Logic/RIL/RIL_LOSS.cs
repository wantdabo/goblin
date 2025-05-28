using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL.Common;

namespace Goblin.Gameplay.Logic.RIL
{
    /// <summary>
    /// LOSS 丢弃渲染指令
    /// </summary>
    public class RIL_LOSS : IRIL
    {
        public override ushort id => RIL_DEFINE.LOSS;
        
        /// <summary>
        /// 丢弃的渲染指令 ID
        /// </summary>
        public ushort loss { get; set; }
        
        protected override void OnReady()
        {
            loss = ushort.MaxValue;
        }

        protected override void OnReset()
        {
            loss = ushort.MaxValue;
        }
    }
}