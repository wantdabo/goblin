using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Logic.Translators.Common;

namespace Goblin.Gameplay.Logic.Translators
{
    /// <summary>
    /// 驱动翻译器
    /// </summary>
    public class TickerTranslator : Translator<TickerInfo>
    {
        protected override void OnRIL(TickerInfo info, int hashcode)
        {
            // ticker.timescale 依赖 stage.timescale
            hashcode += hashcode * 31 + stage.timescale.GetHashCode();
            
            if (stage.rilsync.Query(info.id, RIL_DEFINE.TICKER).Equals(hashcode)) return;
            stage.rilsync.CacheHashCode(info.id, RIL_DEFINE.TICKER, hashcode);

            var ril = ObjectCache.Get<RIL_TICKER>();
            ril.Ready(info.id, hashcode);
            ril.timescale = (stage.timescale * info.timescale * stage.cfg.fp2int).AsUInt();
            stage.rilsync.Send(ril);
        }
    }
}