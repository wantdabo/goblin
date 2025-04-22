using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Logic.RIL.Common;
using Goblin.Gameplay.Render.Resolvers.Common;

namespace Goblin.Gameplay.Render.Resolvers
{
    public class TickerResolver : Resolver<RIL_TICKER>
    {
        public override ushort id => RIL_DEFINE.TICKER;

        protected override void OnRIL(RILState rilstate, RIL_TICKER ril)
        {
        }
    }
}