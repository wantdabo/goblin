using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Logic.Translators.Common;

namespace Goblin.Gameplay.Logic.Translators
{
    /// <summary>
    /// 驱动翻译器
    /// </summary>
    public class Ticker : Translator<TickerInfo>
    {
        protected override void OnRIL(TickerInfo info)
        {
            stage.rilsync.Send(RIL_DEFINE.TYPE_RENDER, info.id, new RIL_TICKER
            {
                timescale = (stage.timescale * info.timescale * stage.cfg.FP2Int).AsUInt()
            });
        }
    }
}