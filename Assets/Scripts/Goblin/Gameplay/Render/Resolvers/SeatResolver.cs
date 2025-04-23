using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Logic.RIL.Common;
using Goblin.Gameplay.Render.Resolvers.Common;
using Goblin.Gameplay.Render.Resolvers.States;

namespace Goblin.Gameplay.Render.Resolvers
{
    public class SeatResolver : Resolver<RIL_SEAT>
    {
        protected override State OnRIL(RILState<RIL_SEAT> rilstate)
        {
            var state = ObjectCache.Get<SeatState>();
            state.seatdict = rilstate.ril.seatdict;
            
            return state;
        }
    }
}