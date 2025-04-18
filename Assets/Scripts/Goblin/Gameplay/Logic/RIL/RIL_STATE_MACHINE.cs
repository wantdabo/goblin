using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL.Common;

namespace Goblin.Gameplay.Logic.RIL
{
    /// <summary>
    /// 状态机渲染指令
    /// </summary>
    public struct RIL_STATE_MACHINE : IRIL
    {
        public ushort id => RIL_DEFINE.STATE_MACHINE;
        /// <summary>
        /// 当前状态
        /// </summary>
        public byte current { get; set; }
        /// <summary>
        /// 上一个状态
        /// </summary>
        public byte last { get; set; }
        /// <summary>
        /// 持续帧数
        /// </summary>
        public uint frames { get; set; }
        /// <summary>
        /// 流逝时间
        /// </summary>
        public uint elapsed { get; set; }
        
        public byte[] Serialize()
        {
            throw new System.NotImplementedException();
        }

        public bool Equals(IRIL other)
        {
            RIL_STATE_MACHINE ril = (RIL_STATE_MACHINE)other;
            
            return ril.id == id && ril.current == current && ril.frames == frames && ril.elapsed == elapsed;
        }

        public override string ToString()
        {
            return $"RIL_STATE_MACHINE: current={current}, last={last}, frames={frames}, elapsed={elapsed}";
        }
    }
}