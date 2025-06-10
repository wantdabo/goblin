using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL.Common;

namespace Goblin.Gameplay.Logic.RIL.DIFF
{
    /// <summary>
    /// 标签渲染指令差值
    /// </summary>
    public class RIL_DIFF_TAG : IRIL_DIFF
    {
        public override ushort id => RIL_DEFINE.TAG;
        
        /// <summary>
        /// 标签的键
        /// </summary>
        public ushort key { get; set; }
        /// <summary>
        /// 标签的值
        /// </summary>
        public int tag { get; set; }
        
        protected override void OnReady()
        {
            key = 0;
            tag = 0;
        }

        protected override void OnReset()
        {
            key = 0;
            tag = 0;
        }

        protected override void OnClone(IRIL_DIFF clone)
        {
            if (clone is not RIL_DIFF_TAG diff) return;
            
            diff.key = key;
            diff.tag = tag;
        }
    }
}