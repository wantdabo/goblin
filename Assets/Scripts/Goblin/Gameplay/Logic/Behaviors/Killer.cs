using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.Behaviors
{
    /// <summary>
    /// 杀手行为
    /// </summary>
    public class Killer : Behavior
    {
        /// <summary>
        /// 击杀关系, 键为杀手, 值为被杀者
        /// </summary>
        private Dictionary<ulong, List<ulong>> killrelations { get; set; }
        /// <summary>
        /// 已被击杀列表
        /// </summary>
        private List<ulong> killeds { get; set; }

        protected override void OnAssemble()
        {
            base.OnAssemble();
            killrelations = ObjectCache.Ensure<Dictionary<ulong, List<ulong>>>();
            killeds = ObjectCache.Ensure<List<ulong>>();
        }

        protected override void OnDisassemble()
        {
            base.OnDisassemble();
            foreach (var kv in killrelations)
            {
                kv.Value.Clear();
                ObjectCache.Set(kv.Value);
            }
            killrelations.Clear();
            ObjectCache.Set(killrelations);
            
            killeds.Clear();
            ObjectCache.Set(killeds);
        }

        /// <summary>
        /// 击杀
        /// </summary>
        /// <param name="killer">杀手 ID</param>
        /// <param name="victim">被杀者 ID</param>
        public void Kill(ulong killer, ulong victim)
        {
            if (killeds.Contains(victim)) return;
            if (false == killrelations.TryGetValue(killer, out var victims)) killrelations.Add(killer, victims = ObjectCache.Ensure<List<ulong>>());
            victims.Add(victim);
        }

        /// <summary>
        /// 查询击杀关系
        /// </summary>
        /// <param name="killer">杀手 ID</param>
        /// <param name="victims">被杀者列表</param>
        /// <returns>YES/NO</returns>
        public bool Query(ulong killer, out List<ulong> victims)
        {
            victims = default;
            if (false == killrelations.TryGetValue(killer, out victims)) return false;
            
            return true;
        }

        protected override void OnEndTick()
        {
            base.OnEndTick();
            // 清空击杀列表
            foreach (var kv in killrelations)
            {
                kv.Value.Clear();
                ObjectCache.Set(kv.Value);
            }
            killrelations.Clear();
            
            killeds.Clear();
        }
    }
}