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
        
        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 31 + id.GetHashCode();
            hash = hash * 31 + current.GetHashCode();
            hash = hash * 31 + last.GetHashCode();
            hash = hash * 31 + frames.GetHashCode();
            hash = hash * 31 + elapsed.GetHashCode();
            
            return hash;
        }

        public override string ToString()
        {
            return $"RIL_STATE_MACHINE: current={current}, last={last}, frames={frames}, elapsed={elapsed}";
        }
    }
}