using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Logic.RIL.Common;
using Goblin.Gameplay.Render.Resolvers.Common;
using Goblin.Gameplay.Render.Resolvers.States;

namespace Goblin.Gameplay.Render.Resolvers
{
    public class StateMachineResolver : Resolver<RIL_STATE_MACHINE>
    {
        protected override IState OnRIL(RILState rilstate, RIL_STATE_MACHINE ril)
        {
            return new StateMachineState
            {
                current = ril.current,
                last = ril.last,
                frames = ril.frames,
                elapsed = ril.elapsed,
            };
        }
    }
}