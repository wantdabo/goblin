using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.BehaviorInfos.Sa;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Logic.Translators.Common;

namespace Goblin.Gameplay.Logic.Translators
{
    /// <summary>
    /// 座位信息翻译器
    /// </summary>
    public class SeatTranslator : Translator<SeatInfo, RIL_SEAT>
    {
        public override ushort id => RIL_DEFINE.SEAT;

        protected override int OnCalcHashCode(SeatInfo info)
        {
            int hash = 17;
            hash = hash * 31 + info.actor.GetHashCode();
            foreach (var kv in info.sadict)
            {
                hash = hash * 31 + kv.Key.GetHashCode();
                hash = hash * 31 + kv.Value.GetHashCode();
            }
            return hash;
        }

        protected override void OnRIL(SeatInfo info, RIL_SEAT ril)
        {
            foreach (var kv in info.sadict) ril.seatdict.Add(kv.Key, kv.Value);
        }
    }
}