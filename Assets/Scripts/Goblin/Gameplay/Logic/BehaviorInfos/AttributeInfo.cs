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
        /// 属性
        /// </summary>
        public Dictionary<ushort, int> datas { get; set; }
        
        protected override void OnReady()
        {
            datas = ObjectCache.Ensure<Dictionary<ushort, int>>();
        }

        protected override void OnReset()
        {
            datas.Clear();
            ObjectCache.Set(datas);
        }

        protected override BehaviorInfo OnClone()
        {
            var clone = ObjectCache.Ensure<AttributeInfo>();
            clone.Ready(actor);
            foreach (var kv in datas) clone.datas.Add(kv.Key, kv.Value);
            
            return clone;
        }
    }
}