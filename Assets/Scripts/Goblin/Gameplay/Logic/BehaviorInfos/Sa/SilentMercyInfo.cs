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
        /// 出生列表
        /// </summary>
        public List<(ulong actor, ulong flow)> borns { get; set; }
        /// <summary>
        /// 死亡列表
        /// </summary>
        public List<(ulong actor, ulong flow)> deadths { get; set; }
        /// <summary>
        /// 击杀关系, 键为杀手, 值为被杀者
        /// </summary>
        public Dictionary<ulong, List<ulong>> killrelations { get; set; }
        /// <summary>
        /// 受害者关系, 键为被杀者, 值为杀手
        /// </summary>
        public Dictionary<ulong, ulong> victimrelations { get; set; }
        
        protected override void OnReady()
        {
            borns = ObjectCache.Ensure<List<(ulong, ulong)>>();            
            deadths = ObjectCache.Ensure<List<(ulong, ulong)>>();
            killrelations = ObjectCache.Ensure<Dictionary<ulong, List<ulong>>>();
            victimrelations = ObjectCache.Ensure<Dictionary<ulong, ulong>>();
        }

        protected override void OnReset()
        {
            borns.Clear();
            ObjectCache.Set(borns);
            
            deadths.Clear();
            ObjectCache.Set(deadths);
            
            foreach (var kv in killrelations)
            {
                kv.Value.Clear();
                ObjectCache.Set(kv.Value);
            }
            killrelations.Clear();
            ObjectCache.Set(killrelations);
            
            victimrelations.Clear();
            ObjectCache.Set(victimrelations);
        }

        protected override BehaviorInfo OnClone()
        {
            var clone = ObjectCache.Ensure<SilentMercyInfo>();
            clone.Ready(actor);
            clone.borns.AddRange(borns);
            clone.deadths.AddRange(deadths);
            foreach (var kv in killrelations)
            {
                var victims = ObjectCache.Ensure<List<ulong>>();
                victims.AddRange(kv.Value);
                clone.killrelations.Add(kv.Key, victims);
            }
            
            foreach (var kv in victimrelations)
            {
                clone.victimrelations.Add(kv.Key, kv.Value);
            }
            
            return clone;
        }
    }
}