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
    public class TickerTranslator : Translator<TickerInfo, RIL_TICKER>
    {
        public override ushort id => RIL_DEFINE.TICKER;

        protected override int OnCalcHashCode(TickerInfo info)
        {
            // ticker.timescale 依赖 stage.timescale
            return base.OnCalcHashCode(info) * 31 + stage.timescale.GetHashCode();
        }

        protected override void OnRIL(TickerInfo info, RIL_TICKER ril)
        {
            ril.timescale = (stage.timescale * info.timescale * stage.cfg.fp2int).AsUInt();
        }
    }
}