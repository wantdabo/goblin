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
        public override ushort id => RIL_DEFINE.TAG;
        
        protected override IState OnRIL(RILState rilstate, RIL_TAG ril)
        {
            if (statebucket.GetState<TagState>(rilstate.actor, StateType.Tag, out var state))
            {
                state.tags.Clear();
                ObjectCache.Set(state.tags);
            }

            return new TagState
            {
                tags = ril.tags,
            };
        }
    }
}