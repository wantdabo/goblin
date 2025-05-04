using Goblin.Common;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Logic.RIL.Common;
using Goblin.Gameplay.Render.Resolvers.Common;
using Goblin.Gameplay.Render.Resolvers.States;

namespace Goblin.Gameplay.Render.Resolvers
{
    /// <summary>
    /// 驱动数据解析器
    /// </summary>
    public class TickerResolver : Resolver<RIL_TICKER, TickerState>
    {
        protected override TickerState OnRIL(RILState<RIL_TICKER> rilstate)
        {
            var state = ObjectCache.Get<TickerState>();
            state.timescale = rilstate.ril.timescale * Config.Int2Float;

            return state;
        }
    }
}