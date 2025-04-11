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
        /// Stage 当前的帧号
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
        
        /// <summary>
        /// 梗概渲染指令
        /// </summary>
        /// <param name="frame">Stage 当前的帧号</param>
        /// <param name="actorcnt">Actor 数量</param>
        /// <param name="behaviorcnt">Behavior 数量</param>
        /// <param name="behaviorinfocnt">BehaviorInfo 数量</param>
        /// <param name="hassnapshot">是否有快照</param>
        /// <param name="snapshotframe">快照帧号</param>
        public RIL_SYNOPSIS(uint frame, uint actorcnt, uint behaviorcnt, uint behaviorinfocnt, bool hassnapshot, uint snapshotframe)
        {
            this.actorcnt = actorcnt;
            this.behaviorcnt = behaviorcnt;
            this.behaviorinfocnt = behaviorinfocnt;
            this.frame = frame;
            this.hassnapshot = hassnapshot;
            this.snapshotframe = snapshotframe;
        }
        
        public byte[] Serialize()
        {
            throw new System.NotImplementedException();
        }

        public bool Equals(IRIL other)
        {
            RIL_SYNOPSIS ril = (RIL_SYNOPSIS) other;
            
            return ril.id == id &&
                   ril.actorcnt == actorcnt &&
                   ril.behaviorcnt == behaviorcnt &&
                   ril.behaviorinfocnt == behaviorinfocnt &&
                   ril.frame == frame &&
                   ril.hassnapshot == hassnapshot &&
                   ril.snapshotframe == snapshotframe;
        }

        public override string ToString()
        {
            return $"RIL_SYNOPSIS: actorcnt={actorcnt}, behaviorcnt={behaviorcnt}, behaviorinfocnt={behaviorinfocnt}, frame={frame}, hassnapshot={hassnapshot}, snapshotframe={snapshotframe}";
        }
    }
}