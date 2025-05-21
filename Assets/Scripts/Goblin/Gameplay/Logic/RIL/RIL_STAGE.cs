using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL.Common;

namespace Goblin.Gameplay.Logic.RIL
{
    /// <summary>
    /// 场景渲染指令
    /// </summary>
    public class RIL_STAGE : IRIL
    {
        public override ushort id => RIL_DEFINE.STAGE;
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

        protected override void OnReady()
        {
            frame = 0;
            actorcnt = 0;
            behaviorcnt = 0;
            behaviorinfocnt = 0;
            hassnapshot = false;
            snapshotframe = 0;
        }

        protected override void OnReset()
        {
            frame = 0;
            actorcnt = 0;
            behaviorcnt = 0;
            behaviorinfocnt = 0;
            hassnapshot = false;
            snapshotframe = 0;
        }

        protected override void OnCopy(IRIL target)
        {
            if (target is not RIL_STAGE ril) return;
            ril.frame = frame;
            ril.actorcnt = actorcnt;
            ril.behaviorcnt = behaviorcnt;
            ril.behaviorinfocnt = behaviorinfocnt;
            ril.hassnapshot = hassnapshot;
            ril.snapshotframe = snapshotframe;
        }

        public override byte[] Serialize()
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return $"RIL_SYNOPSIS: frame={frame}, actorcnt={actorcnt}, behaviorcnt={behaviorcnt}, behaviorinfocnt={behaviorinfocnt}, hassnapshot={hassnapshot}, snapshotframe={snapshotframe}";
        }
    }
}