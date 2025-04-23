using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL.Common;

namespace Goblin.Gameplay.Logic.RIL
{
    /// <summary>
    /// 标签渲染指令
    /// </summary>
    public struct RIL_TAG : IRIL
    {
        public ushort id => RIL_DEFINE.TAG;
        
        /// <summary>
        /// 标签的数据集合, 键为 TAG_DEFINE, 值为 Int32
        /// </summary>
        public Dictionary<ushort, int> tags { get; set; }

        public byte[] Serialize()
        {
            throw new System.NotImplementedException();
        }

        public bool Equals(IRIL other)
        {
            RIL_TAG ril = (RIL_TAG)other;
            if (null == tags) return false;
            if (null == ril.tags) return false;
            
            if (tags.Count != ril.tags.Count) return false;
            foreach (var kv in tags)
            {
                if (false == ril.tags.TryGetValue(kv.Key, out var value) || value != kv.Value)
                {
                    return false;
                }
            }
            
            return true;
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