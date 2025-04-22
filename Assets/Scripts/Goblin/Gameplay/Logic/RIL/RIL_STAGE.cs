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

        public bool Equals(IRIL other)
        {
            RIL_STAGE ril = (RIL_STAGE) other;
            
            return ril.id == id &&
                   ril.actorcnt == actorcnt &&
                   ril.behaviorcnt == behaviorcnt &&
                   ril.behaviorinfocnt == behaviorinfocnt &&
                   ril.hassnapshot == hassnapshot &&
                   ril.snapshotframe == snapshotframe;
        }

        public override string ToString()
        {
            return $"RIL_SYNOPSIS: actorcnt={actorcnt}, behaviorcnt={behaviorcnt}, behaviorinfocnt={behaviorinfocnt}, hassnapshot={hassnapshot}, snapshotframe={snapshotframe}";
        }
    }
}