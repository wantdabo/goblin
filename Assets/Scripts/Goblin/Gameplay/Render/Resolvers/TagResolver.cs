using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Logic.RIL.Common;
using Goblin.Gameplay.Render.Resolvers.Common;
using Goblin.Gameplay.Render.Resolvers.States;

namespace Goblin.Gameplay.Render.Resolvers
{
    public class TagResolver : Resolver<RIL_TAG>
    {
        protected override State OnRIL(RILState<RIL_TAG> rilstate)
        {
            var state = ObjectCache.Get<TagState>();
            state.tags = rilstate.ril.tags;

            return state;
        }
    }
}