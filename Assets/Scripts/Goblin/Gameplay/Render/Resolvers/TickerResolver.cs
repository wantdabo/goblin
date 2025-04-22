using Goblin.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Logic.RIL.Common;
using Goblin.Gameplay.Render.Resolvers.Common;
using Goblin.Gameplay.Render.Resolvers.States;

namespace Goblin.Gameplay.Render.Resolvers
{
    public class TickerResolver : Resolver<RIL_TICKER>
    {
        public override ushort id => RIL_DEFINE.TICKER;

        protected override IState OnRIL(RILState rilstate, RIL_TICKER ril)
        {
            return new TickerState
            {
                actor = rilstate.actor,
                frame = rilstate.frame,
                timescale = ril.timescale * Config.Int2Float,
            };
        }
    }
}