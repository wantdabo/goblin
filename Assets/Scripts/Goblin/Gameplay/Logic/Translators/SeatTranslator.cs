using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Logic.Translators.Common;

namespace Goblin.Gameplay.Logic.Translators
{
    /// <summary>
    /// 座位信息翻译器
    /// </summary>
    public class SeatTranslator : Translator<SeatInfo>
    {
        protected override void OnRIL(SeatInfo info, int hashcode)
        {
            if (stage.rilsync.Query(info.id, RIL_DEFINE.SEAT).Equals(hashcode)) return;
            stage.rilsync.CacheHashCode(info.id, RIL_DEFINE.SEAT, hashcode);
            
            var ril = ObjectCache.Get<RIL_SEAT>();
            ril.Ready(info.id, hashcode);
            foreach (var kv in info.sadict) ril.seatdict.Add(kv.Key, kv.Value);
            stage.rilsync.Send(ril);
        }
    }
}