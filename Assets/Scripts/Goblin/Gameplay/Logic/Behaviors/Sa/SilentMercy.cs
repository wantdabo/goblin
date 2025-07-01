using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos.Sa;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.Behaviors.Sa
{
    /// <summary>
    /// 生与死行为
    /// </summary>
    public class SilentMercy : Behavior<SilentMercyInfo>
    {
        /// <summary>
        /// 出生
        /// </summary>
        /// <param name="actor">ActorID</param>
        public void Born(ulong actor)
        {
        }

        /// <summary>
        /// 死亡
        /// </summary>
        /// <param name="actor">ActorID</param>
        public void Dead(ulong actor)
        {
        }
        
        /// <summary>
        /// 击杀
        /// </summary>
        /// <param name="killer">杀手 ID</param>
        /// <param name="victim">被杀者 ID</param>
        public void Kill(ulong killer, ulong victim)
        {
            if (info.victimrelations.ContainsKey(victim)) return;
            if (false == info.killrelations.TryGetValue(killer, out var victims)) info.killrelations.Add(killer, victims = ObjectCache.Ensure<List<ulong>>());
            victims.Add(victim);
            info.victimrelations.Add(victim, killer);
            
            Dead(actor);
        }

        /// <summary>
        /// 查询击杀关系
        /// </summary>
        /// <param name="killer">杀手 ID</param>
        /// <param name="victims">被杀者列表</param>
        /// <returns>YES/NO</returns>
        public bool AskKiller(ulong killer, out List<ulong> victims)
        {
            victims = default;
            if (false == info.killrelations.TryGetValue(killer, out victims)) return false;
            
            return true;
        }
        
        /// <summary>
        /// 查询被杀关系
        /// </summary>
        /// <param name="victim">被杀者 ID</param>
        /// <param name="killer">杀手 ID</param>
        /// <returns>YES/NO</returns>
        public bool AskVictim(ulong victim, out ulong killer)
        {
            killer = default;
            if (false == info.victimrelations.TryGetValue(victim, out killer)) return false;
            
            return true;
        }

        protected override void OnEndTick()
        {
            base.OnEndTick();
            // 清空击杀列表
            foreach (var kv in info.killrelations)
            {
                kv.Value.Clear();
                ObjectCache.Set(kv.Value);
            }
            info.killrelations.Clear();
        }
    }
}