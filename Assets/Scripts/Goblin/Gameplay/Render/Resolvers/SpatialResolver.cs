using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Render.Resolvers.Common;

namespace Goblin.Gameplay.Render.Resolvers
{
    public class SpatialResolver : Resolver<RIL_SPATIAL>
    {
        public override ushort id => RIL_DEFINE.SPATIAL;
        
        protected override void OnRIL(RIL_SPATIAL ril)
        {
        }
    }
}