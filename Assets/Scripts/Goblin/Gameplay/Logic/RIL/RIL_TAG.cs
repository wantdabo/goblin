using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL.Common;

namespace Goblin.Gameplay.Logic.RIL
{
    /// <summary>
    /// 标签渲染指令
    /// </summary>
    public class RIL_TAG : IRIL
    {
        public override ushort id => RIL_DEFINE.TAG;
        
        /// <summary>
        /// 标签的数据集合, 键为 TAG_DEFINE, 值为 Int32
        /// </summary>
        public Dictionary<ushort, int> tags { get; set; }

        protected override void OnReady()
        {
            tags = ObjectCache.Ensure<Dictionary<ushort, int>>();
        }

        protected override void OnReset()
        {
            tags.Clear();
            ObjectCache.Set(tags);
        }

        protected override void OnCopy(IRIL target)
        {
            if (target is not RIL_TAG ril) return;
            foreach (var kv in tags) ril.tags.Add(kv.Key, kv.Value);
        }

        public override byte[] Serialize()
        {
            throw new System.NotImplementedException();
        }
        
        public override string ToString()
        {
            if (null == tags) return "RIL_TAG: tags=null";
            if (tags.Count == 0) return "RIL_TAG: tags={}";
            
            string result = "RIL_TAG: tags={";
            foreach (var kv in tags)
            {
                result += $" {kv.Key}={kv.Value},";
            }
            result += " }";
            return result;
        }
    }
}