using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Logic.RIL.Common;
using Goblin.Gameplay.Render.Resolvers.Common;
using Goblin.Gameplay.Render.Resolvers.States;

namespace Goblin.Gameplay.Render.Resolvers
{
    public class StateMachineResolver : Resolver<RIL_STATE_MACHINE, StateMachineState>
    {
        protected override StateMachineState OnRIL(RILState<RIL_STATE_MACHINE> rilstate)
        {
            var state = ObjectCache.Get<StateMachineState>();
            state.current = rilstate.ril.current;
            state.last = rilstate.ril.last;
            state.frames = rilstate.ril.frames;
            state.elapsed = rilstate.ril.elapsed;

            return state;
        }
    }
}