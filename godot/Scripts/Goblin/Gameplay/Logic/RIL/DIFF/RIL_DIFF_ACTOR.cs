using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL.Common;

namespace Goblin.Gameplay.Logic.RIL.DIFF
{
    /// <summary>
    /// Actor 渲染指令差值
    /// </summary>
    public class RIL_DIFF_ACTOR : IRIL_DIFF
    {
        public override ushort id => RIL_DEFINE.ACTOR;
        
        /// <summary>
        /// ActorID
        /// </summary>
        public ulong target { get; set; }
        
        protected override void OnReady()
        {
            target = 0;
        }

        protected override void OnReset()
        {
            target = 0;
        }

        protected override void OnClone(IRIL_DIFF clone)
        {
            if (clone is not RIL_DIFF_ACTOR diff) return;
            diff.target = target;
        }
    }
}