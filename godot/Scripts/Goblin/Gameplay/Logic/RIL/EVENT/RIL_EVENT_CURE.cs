using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL.Common;

namespace Goblin.Gameplay.Logic.RIL.EVENT
{
    /// <summary>
    /// RIL 事件 - 治疗事件
    /// </summary>
    public class RIL_EVENT_CURE : IRIL_EVENT
    {
        public override ushort id => RIL_DEFINE.EVENT_CURE;

        /// <summary>
        /// 来源, ActorID
        /// </summary>
        public ulong from { get; set; }
        /// <summary>
        /// 去向, ActorID
        /// </summary>
        public ulong to { get; set; }
        /// <summary>
        /// 治疗值
        /// </summary>
        public int cure { get; set; }

        protected override void OnReset()
        {
            from = 0;
            to = 0;
            cure = 0;
        }

        protected override void OnClone(IRIL_EVENT clone)
        {
            if (clone is not RIL_EVENT_CURE e) return;
            
            e.from = from;
            e.to = to;
            e.cure = cure;
        }
    }
}