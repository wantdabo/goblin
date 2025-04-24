using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL.Common;

namespace Goblin.Gameplay.Logic.RIL
{
    /// <summary>
    /// 场景渲染指令
    /// </summary>
    public struct RIL_STAGE : IRIL
    {
        public ushort id => RIL_DEFINE.STAGE;
        /// <summary>
        /// 帧号
        /// </summary>
        public uint frame { get; set; }
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
        
        public override int GetHashCode()
        {
            return frame.GetHashCode() ^ actorcnt.GetHashCode() ^ behaviorcnt.GetHashCode() ^ behaviorinfocnt.GetHashCode() ^ hassnapshot.GetHashCode() ^ snapshotframe.GetHashCode();
        }

        public override string ToString()
        {
            return $"RIL_SYNOPSIS: frame={frame}, actorcnt={actorcnt}, behaviorcnt={behaviorcnt}, behaviorinfocnt={behaviorinfocnt}, hassnapshot={hassnapshot}, snapshotframe={snapshotframe}";
        }
    }
}