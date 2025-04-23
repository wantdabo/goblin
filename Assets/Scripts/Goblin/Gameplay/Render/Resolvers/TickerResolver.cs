using Goblin.Common;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Logic.RIL.Common;
using Goblin.Gameplay.Render.Resolvers.Common;
using Goblin.Gameplay.Render.Resolvers.States;

namespace Goblin.Gameplay.Render.Resolvers
{
    public class TickerResolver : Resolver<RIL_TICKER>
    {
        protected override State OnRIL(RILState<RIL_TICKER> rilstate)
        {
            var state = ObjectCache.Get<TickerState>();
            state.timescale = rilstate.ril.timescale * Config.Int2Float;

            return state;
        }
    }
}