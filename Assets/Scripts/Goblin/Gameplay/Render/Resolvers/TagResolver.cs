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
            return new TagState
            {
                actor = rilstate.actor,
                frame = rilstate.frame,
                actortype = ril.actortype,
                hashero = ril.hashero,
                hero = ril.hero,
                model = ril.model
            };
        }
    }
}