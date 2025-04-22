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
        public override ushort id => RIL_DEFINE.SEAT;

        protected override IState OnRIL(RILState rilstate, RIL_SEAT ril)
        {
            if (statebucket.GetState<SeatState>(rilstate.actor, StateType.Seat, out var state))
            {
                state.seatdict.Clear();
                ObjectCache.Set(state.seatdict);
            }

            return new SeatState
            {
                seatdict = ril.seatdict,
            };
        }
    }
}