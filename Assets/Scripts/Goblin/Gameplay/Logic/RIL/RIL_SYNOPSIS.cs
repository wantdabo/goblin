using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL.Common;

namespace Goblin.Gameplay.Logic.RIL
{
    /// <summary>
    /// 梗概渲染指令
    /// </summary>
    public struct RIL_SYNOPSIS : IRIL
    {
        public ushort id => RIL_DEFINE.SYNOPSIS;
        
        /// <summary>
        /// Actor 数量
        /// </summary>
        public uint actorcnt { get; set; }
        /// <summary>
        /// Behavior 数量
        /// </summary>
        public uint behaviorcnt { get; set; }
        /// <summary>
        /// BehaviorInfo 数量
        /// </summary>
        public uint behaviorinfocnt { get; set; }
        /// <summary>
        /// Stage 当前的帧号
        /// </summary>
        public uint frame { get; set; }
        /// <summary>
        /// 是否有快照
        /// </summary>
        public bool hassnapshot { get; set; }
        /// <summary>
        /// 快照帧号
        /// </summary>
        public uint snapshotframe { get; set; }
        
        public byte[] Serialize()
        {
            throw new System.NotImplementedException();
        }

        public bool Equals(IRIL other)
        {
            throw new System.NotImplementedException();
        }
    }
}