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
        public byte current { get; private set; }
        /// <summary>
        /// 持续帧
        /// </summary>
        public uint frames { get; private set; }
        
        /// <summary>
        /// 状态机渲染指令
        /// </summary>
        /// <param name="current">当前状态</param>
        /// <param name="frames">持续帧</param>
        public RIL_STATE_MACHINE(byte current, uint frames)
        {
            this.current = current;
            this.frames = frames;
        }
        
        public byte[] Serialize()
        {
            throw new System.NotImplementedException();
        }

        public bool Equals(IRIL other)
        {
            if (other is RIL_STATE_MACHINE _other)
            {
                return current == _other.current && frames == _other.frames;
            }
            
            return false;
        }

        public override string ToString()
        {
            return $"RIL_STATE_MACHINE: current={current}, frames={frames}";
        }
    }
}