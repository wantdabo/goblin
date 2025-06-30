using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.BehaviorInfos.Sa
{
    /// <summary>
    /// 生与死信息
    /// </summary>
    public class SilentMercyInfo : BehaviorInfo
    {
        /// <summary>
        /// 已被击杀列表
        /// </summary>
        public List<ulong> deadths { get; set; }
        /// <summary>
        /// 击杀关系, 键为杀手, 值为被杀者
        /// </summary>
        public Dictionary<ulong, List<ulong>> killrelations { get; set; }
        
        protected override void OnReady()
        {
            deadths = ObjectCache.Ensure<List<ulong>>();
            killrelations = ObjectCache.Ensure<Dictionary<ulong, List<ulong>>>();
        }

        protected override void OnReset()
        {
            foreach (var kv in killrelations)
            {
                kv.Value.Clear();
                ObjectCache.Set(kv.Value);
            }
            killrelations.Clear();
            ObjectCache.Set(killrelations);
            
            deadths.Clear();
            ObjectCache.Set(deadths);
        }

        protected override BehaviorInfo OnClone()
        {
            var clone = ObjectCache.Ensure<SilentMercyInfo>();
            clone.Ready(actor);
            foreach (var kv in killrelations)
            {
                var victims = ObjectCache.Ensure<List<ulong>>();
                victims.AddRange(kv.Value);
                clone.killrelations.Add(kv.Key, victims);
            }
            clone.deadths.AddRange(deadths);
            
            return clone;
        }
    }
}