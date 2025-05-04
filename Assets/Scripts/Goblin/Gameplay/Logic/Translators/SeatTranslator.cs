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
        protected override void OnRIL(SeatInfo info)
        {
            RIL_SEAT ril = new RIL_SEAT();
            ril.seatdict = ObjectCache.Get<Dictionary<ulong, ulong>>();
            foreach (var kv in info.sadict) ril.seatdict.Add(kv.Key, kv.Value);
            stage.rilsync.Send(RIL_DEFINE.TYPE_RENDER, info.id, ril);
        }
    }
}