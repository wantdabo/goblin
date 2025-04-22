using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Logic.RIL.Common;
using Goblin.Gameplay.Render.Common.Extensions;
using Goblin.Gameplay.Render.Resolvers.Common;
using Goblin.Gameplay.Render.Resolvers.States;

namespace Goblin.Gameplay.Render.Resolvers
{
    public class SpatialResolver : Resolver<RIL_SPATIAL>
    {
        public override ushort id => RIL_DEFINE.SPATIAL;

        protected override void OnRIL(RILState rilstate, RIL_SPATIAL ril)
        {
            SpatialState state = new()
            {
                frame = rilstate.frame,
                actor = rilstate.actor,
                position = ril.position.ToVector3(),
                euler = ril.euler.ToVector3(),
                scale = ril.scale.ToVector3(),
            };
            // statebucket.SetState();
        }
    }
}