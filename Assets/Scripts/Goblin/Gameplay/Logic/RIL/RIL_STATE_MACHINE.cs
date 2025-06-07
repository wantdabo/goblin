using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL.Common;

namespace Goblin.Gameplay.Logic.RIL
{
    /// <summary>
    /// 状态机渲染指令
    /// </summary>
    public class RIL_STATE_MACHINE : IRIL
    {
        public override ushort id => RIL_DEFINE.STATE_MACHINE;
        /// <summary>
        /// 当前状态
        /// </summary>
        public byte current { get; set; }
        /// <summary>
        /// 上一个状态
        /// </summary>
        public byte last { get; set; }
        /// <summary>
        /// 流逝时间
        /// </summary>
        public uint elapsed { get; set; }

        protected override void OnReady()
        {
            current = 0;
            last = 0;
            elapsed = 0;
        }

        protected override void OnReset()
        {
            current = 0;
            last = 0;
            elapsed = 0;
        }
        
        public override string ToString()
        {
            return $"RIL_STATE_MACHINE: current={current}, last={last}, elapsed={elapsed}";
        }
    }
}