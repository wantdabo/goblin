using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.BehaviorInfos
{
    /// <summary>
    /// 属性信息
    /// </summary>
    public class AttributeInfo : BehaviorInfo
    {
        /// <summary>
        /// 基础属性
        /// </summary>
        public Dictionary<ushort, int> baseattributes { get; set; }
        /// <summary>
        /// 附加属性
        /// </summary>
        public Dictionary<ushort, int> addiattributes { get; set; }
        
        protected override void OnReady()
        {
            baseattributes = ObjectCache.Ensure<Dictionary<ushort, int>>();
            addiattributes = ObjectCache.Ensure<Dictionary<ushort, int>>();
        }

        protected override void OnReset()
        {
            baseattributes.Clear();
            ObjectCache.Set(baseattributes);
            
            addiattributes.Clear();
            ObjectCache.Set(addiattributes);
        }

        protected override BehaviorInfo OnClone()
        {
            var clone = ObjectCache.Ensure<AttributeInfo>();
            clone.Ready(actor);
            foreach (var kv in baseattributes) clone.baseattributes.Add(kv.Key, kv.Value);
            foreach (var kv in addiattributes) clone.addiattributes.Add(kv.Key, kv.Value);
            
            return clone;
        }
    }
}