using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.BehaviorInfos
{
    /// <summary>
    /// 座位信息
    /// </summary>
    public class SeatInfo : BehaviorInfo
    {
        /// <summary>
        /// 座位字典, 键为座位 ID, 值为 ActorID
        /// </summary>
        public Dictionary<ulong, ulong> sadict { get; set; }
        /// <summary>
        /// 座位字典, 键为 ActorID, 值为座位 ID
        /// </summary>
        public Dictionary<ulong, ulong> asdict { get; set; }
        
        protected override void OnReady()
        {
            sadict = ObjectCache.Get<Dictionary<ulong, ulong>>();
            asdict = ObjectCache.Get<Dictionary<ulong, ulong>>();
        }

        protected override void OnReset()
        {
            sadict.Clear();
            ObjectCache.Set(sadict);
            
            asdict.Clear();
            ObjectCache.Set(asdict);
        }

        protected override BehaviorInfo OnClone()
        {
            var clone = ObjectCache.Get<SeatInfo>();
            clone.Ready(id);
            foreach (var kv in sadict) clone.sadict.Add(kv.Key, kv.Value);
            foreach (var kv in asdict) clone.asdict.Add(kv.Key, kv.Value);

            return clone;
        }
    }
}