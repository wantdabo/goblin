using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL.Common;

namespace Goblin.Gameplay.Logic.RIL.DIFF
{
    /// <summary>
    /// 标签渲染指令差值指令 
    /// </summary>
    public class RIL_DIFF_TAG : IRIL_DIFF
    {
        public override ushort id => RIL_DEFINE.TAG;
        
        /// <summary>
        /// 标签的数据集合, 键为 TAG_DEFINE, 值为 Int32
        /// </summary>
        public Dictionary<ushort, int> tags { get; set; }
        
        protected override void OnReady()
        {
            tags = RILCache.Ensure<Dictionary<ushort, int>>();
        }

        protected override void OnReset()
        {
            tags.Clear();
            RILCache.Set(tags);
        }

        public override byte[] Serialize()
        {
            throw new System.NotImplementedException();
        }
    }
}