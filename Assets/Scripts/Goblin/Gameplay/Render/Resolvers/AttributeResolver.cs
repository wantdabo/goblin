using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Logic.RIL.Common;
using Goblin.Gameplay.Render.Resolvers.Common;
using Goblin.Gameplay.Render.Resolvers.States;

namespace Goblin.Gameplay.Render.Resolvers
{
    public class AttributeResolver : Resolver<RIL_ATTRIBUTE>
    {
        protected override IState OnRIL(RILState rilstate, RIL_ATTRIBUTE ril)
        {
            return new AttributeState
            {
                hp = ril.hp,
                maxhp = ril.maxhp,
                movespeed = ril.movespeed,
                attack = ril.attack,
            };
        }
    }
}